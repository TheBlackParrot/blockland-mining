function Mining_newBrick(%x,%y,%z) {
	if(!$Mining::Brick[%x,%y,%z]) {
		%brick = new fxDTSBrick(MiningBrick) {
			angleID = 0;
			colorFxID = 0;
			colorID = 0;
			dataBlock = "brick8xCubeData";
			isBasePlate = 1;
			isPlanted = 1;
			position = %x SPC %y SPC %z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			health = getRandom(10,15);
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

	Mining_newBrick(%x+4,%y,%z);
	Mining_newBrick(%x-4,%y,%z);
	Mining_newBrick(%x,%y+4,%z);
	Mining_newBrick(%x,%y-4,%z);
	Mining_newBrick(%x,%y,%z+4);
	Mining_newBrick(%x,%y,%z-4);
}

function fxDTSBrick::mineBrick(%this,%player) {
	talk("Triggered [B]" SPC %this);
	%client = %player.client;
	%this.hits++;
	if(%this.hits >= %this.health) {
		%this.fakeKillBrick();
		%this.playSound(pop_high);

		%this.placeSurroundings();

		%this.schedule(3000,delete);
	} else {
		%this.playSound(pop_low);
	}
	%client.centerPrint(%this.hits,1);
}