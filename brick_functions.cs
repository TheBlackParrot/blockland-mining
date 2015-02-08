function Mining_newBrick(%x,%y,%z,%prev) {
	if(!$Mining::Brick[%x,%y,%z]) {
		%biome = getBiome(%prev || -1);

		%brick = new fxDTSBrick(MiningBrick) {
			angleID = 0;
			colorFxID = 0;
			colorID = %biome.color;
			dataBlock = "brick8xCubeData";
			isBasePlate = 1;
			isPlanted = 1;
			position = %x SPC %y SPC %z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			health = %biome.health + mCeil(getRandom((%biome.health/6)*-1,%biome.health/4));
			type = %biome.type;
			biomeObj = %biome;
		};
		BrickGroup_888888.add(%brick);
		%brick.plant();
		%brick.setTrusted(1);

		$Mining::Brick[%x,%y,%z] = 1;
	}

	return %brick || -1;
}
schedule(100,0,Mining_newBrick,50,50,50);

function fxDTSBrick::placeSurroundings(%this) {
	%x = getWord(%this.getPosition(),0);
	%y = getWord(%this.getPosition(),1);
	%z = getWord(%this.getPosition(),2);

	Mining_newBrick(%x+4,%y,%z,%this);
	Mining_newBrick(%x-4,%y,%z,%this);
	Mining_newBrick(%x,%y+4,%z,%this);
	Mining_newBrick(%x,%y-4,%z,%this);
	Mining_newBrick(%x,%y,%z+4,%this);
	Mining_newBrick(%x,%y,%z-4,%this);
}

function fxDTSBrick::mineBrick(%this,%player) {
	talk("Triggered [B]" SPC %this);
	%client = %player.client;
	%this.hits++;
	if(%this.hits >= %this.health) {
		%this.fakeKillBrick();
		%this.playSound(pop_high);
		%this.hits = %this.health;

		%this.placeSurroundings();

		%this.schedule(3000,delete);
	} else {
		%this.playSound(pop_low);
	}
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