function Mining_newBrick(%x,%y,%z,%prev,%disable_liquid) {
	if(!$Mining::Brick[%x,%y,%z]) {
		%biome = getBiome(%prev || -1);

		%ore = -1;
		%liquid = -1;
		%color = %biome.color;
		%health = %biome.health;
		%type = %biome.type;
		%value = 0;
		%hazardous = 0;
		%datablock = "brick8xCubeData";

		if(isObject(%prev)) {
			if(isObject(%prev.oreObj) && getRandom(0,50) <= 6) {
				%ore = %prev.oreObj;
			} else {
				if(getRandom(0,100) <= 6) {
					%ore = getOre(%prev || -1);
				}
			}
		} else {
			if(getRandom(0,100) <= 6) {
				%ore = getOre(%prev || -1);
			}
		}
		if(isObject(%ore) && %ore != -1 && %ore !$= "") {
			%color = %ore.color;
			%health = %ore.health;
			%type = %ore.type;
			%value = %ore.value;
		} else {
			if(getRandom(0,100) <= 6 && !%disable_liquid) {
				%liquid = getLiquid();
				%color = %liquid.color;
				%type = %liquid.type;
				%hazardous = %liquid.hazardous;
				%datablock = "brick8xWaterData";
			}
		}

		%brick = new fxDTSBrick(MiningBrick) {
			angleID = 0;
			colorFxID = 0;
			colorID = %color;
			dataBlock = %datablock;
			isBasePlate = 1;
			isPlanted = 1;
			position = %x SPC %y SPC %z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			health = %health + mCeil(getRandom((%health/6)*-1,%health/4));
			type = %type;
			value = %value;
			hazardous = %hazardous;
			biomeObj = %biome;
			oreObj = %ore;
			liquidObj = %liquid;
		};
		BrickGroup_888888.add(%brick);
		%brick.plant();
		%brick.setTrusted(1);

		$Mining::Brick[%x,%y,%z] = %brick;
	}

	if(%liquid != -1 && isObject(%brick)) {
		%brick.placeSurroundings();
		if(!%brick.isSurrounded()) {
			%brick.delete();
		} else {
			%brick.createWaterZone();
			%brick.setColliding(0);
			%brick.setRaycasting(1);

			%zone = %brick.PhysicalZone;
			%zone.setWaterColor(getWords(getColorIDTable(%brick.color),0,2) SPC "0.5");
		}
	}
	return %brick || -1;
}

function fxDTSBrick::placeSurroundings(%this,%disable_liquid) {
	%x = getWord(%this.getPosition(),0);
	%y = getWord(%this.getPosition(),1);
	%z = getWord(%this.getPosition(),2);

	Mining_newBrick(%x+4,%y,%z,%this,%disable_liquid);
	Mining_newBrick(%x-4,%y,%z,%this,%disable_liquid);
	Mining_newBrick(%x,%y+4,%z,%this,%disable_liquid);
	Mining_newBrick(%x,%y-4,%z,%this,%disable_liquid);
	Mining_newBrick(%x,%y,%z+4,%this,%disable_liquid);
	Mining_newBrick(%x,%y,%z-4,%this,%disable_liquid);
}

function fxDTSBrick::isSurrounded(%this) {
	%x = getWord(%this.getPosition(),0);
	%y = getWord(%this.getPosition(),1);
	%z = getWord(%this.getPosition(),2);

	%surrounded = 0;
	if(isObject($Mining::Brick[%x+4,%y,%z]) && !$Mining::Brick[%x+4,%y,%z].isFakeDead()) {
		if(isObject($Mining::Brick[%x-4,%y,%z]) && !$Mining::Brick[%x-4,%y,%z].isFakeDead()) {
			if(isObject($Mining::Brick[%x,%y+4,%z]) && !$Mining::Brick[%x,%y+4,%z].isFakeDead()) {
				if(isObject($Mining::Brick[%x,%y-4,%z]) && !$Mining::Brick[%x,%y-4,%z].isFakeDead()) {
					if(isObject($Mining::Brick[%x,%y,%z-4]) && !$Mining::Brick[%x,%y,%z-4].isFakeDead()) {
						%surrounded = 1;
					}
				}
			}
		}
	}
	return %surrounded;
}

function fxDTSBrick::checkSurroundingLiquids(%this) {
	%x = getWord(%this.getPosition(),0);
	%y = getWord(%this.getPosition(),1);
	%z = getWord(%this.getPosition(),2);

	%brick[0] = $Mining::Brick[%x+4,%y,%z];
	%brick[1] = $Mining::Brick[%x-4,%y,%z];
	%brick[2] = $Mining::Brick[%x,%y+4,%z];
	%brick[3] = $Mining::Brick[%x,%y-4,%z];
	%brick[4] = $Mining::Brick[%x,%y,%z-4];
	for(%i=0;%i<5;%i++) {
		if(isObject(%brick[%i])) {
			if(%brick[%i].PhysicalZone) {
				if(!%brick[%i].isSurrounded()) {
					talk("SHOULD DELETE" SPC %brick[%i]);
					%brick[%i].delete();
				}
			}
		}
	}
}

function fxDTSBrick::mineBrick(%this,%player) {
	%client = %player.client;
	%this.hits += %client.level[power];
	if(%this.hits >= %this.health) {
		%this.fakeKillBrick();
		%this.playSound(pop_high);
		%this.hits = %this.health;

		if(isObject(%this.oreObj)) {
			%row = %this.oreObj;
			%client.amount[strLwr(%row.type)]++;
			%client.points += %row.value;
		}

		if(isObject(%this.PhysicalZone)) {
			%this.PhysicalZone.delete();
		}
		%this.checkSurroundingLiquids();

		%this.placeSurroundings();
		%client.saveMiningGame();

		%this.schedule(3000,delete);
	} else {
		%this.playSound(pop_low);
	}
	%client.updateBottomPrint();
	%client.printMiningBrickInfo(%this);
}

function GameConnection::printMiningBrickInfo(%this,%brick) {
	%hits = %brick.hits;
	%max_hits = %brick.health;
	%color = rgbToHex(getColorIDTable(%brick.colorID));
	%str[0] = "<just:left><font:Arial Bold:30><color:" @ %color @ ">" @ %brick.type @ "<just:right>\c6" @ %brick.hits @ "/" @ %brick.health;

	%str[1] = "<br><font:Courier:16><just:center>\c3-\c6";
	%pblen = 70;
	%width = mCeil((%hits/%max_hits)*%pblen);
	for(%i=0;%i<%width;%i++) {
		%add = %add @ "-";
	}
	%add = %add @ "\c7";
	for(%i=0;%i<(%pblen-%width);%i++) {
		%add = %add @ "-";
	}
	%str[1] = %str[1] @ %add @ "\c3-";

	%this.centerPrint(%str[0] @ %str[1],2);
}