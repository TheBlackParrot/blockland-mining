package MiningLiquidPackage {
	function PlayerStandardArmor::onEnterLiquid(%data,%obj,%coverage,%type) {
		talk(%data SPC %obj SPC %coverage SPC %type);
		%obj.schedule(100,doLiquidLoop);
		return parent::onEnterLiquid(%data,%obj,%coverage,%type);
	}
	function PlayerStandardArmor::onLeaveLiquid(%data,%obj,%coverage,%type) {
		cancel(%obj.liquidLoop);
		talk(%data SPC %obj SPC %coverage SPC %type);
		return parent::onLeaveLiquid(%data,%obj,%coverage,%type);
	}
};
activatePackage(MiningLiquidPackage);

function Player::doLiquidLoop(%this) {
	cancel(%this.liquidLoop);
	%this.liquidLoop = %this.schedule(500,doLiquidLoop);

	%player_pos = %this.getPosition();
	%player_pos[x] = getWord(%player_pos,0);
	%player_pos[y] = getWord(%player_pos,1);
	%player_pos[z] = getWord(%player_pos,2);

	// ugh
	// can't %array[%another[1]] and it SUCKS
	if(%player_pos[x] < 0) {
		%player_pos_xf = mCeil(%player_pos[x] - (%player_pos[x] % 4));
	} else {
		%player_pos_xf = mFloor(%player_pos[x] - (%player_pos[x] % 4));
	}
	if(%player_pos[y] < 0) {
		%player_pos_yf = mCeil(%player_pos[y] - (%player_pos[y] % 4));
	} else {
		%player_pos_yf = mFloor(%player_pos[y] - (%player_pos[y] % 4));
	}
	if(%player_pos[z] < 0) {
		%player_pos_zf = mCeil(%player_pos[z] - (%player_pos[z] % 4));
	} else {
		%player_pos_zf = mFloor(%player_pos[z] - (%player_pos[z] % 4));
	}

	%brick = $Mining::Brick[%player_pos_xf,%player_pos_yf,%player_pos_zf];

	if(!isObject(%brick)) {
		talk("NOT OBJECT" SPC %brick SPC "$Mining::Brick[" @ %player_pos_xf @ "," @ %player_pos_yf @ "," @ %player_pos_zf @ "]" SPC %this.getPosition());
		return;
	}

	%brick_box = %brick.getWorldBox();
	%brick_box[x1] = getWord(%brick_box,0);
	%brick_box[y1] = getWord(%brick_box,1);
	%brick_box[z1] = getWord(%brick_box,2);
	%brick_box[x2] = getWord(%brick_box,3);
	%brick_box[y2] = getWord(%brick_box,4);
	%brick_box[z2] = getWord(%brick_box,5);

	if(%player_pos_xf >= %brick_box[x1] && %player_pos_yf >= %brick_box[y1] && %player_pos_zf >= %brick_box[z1] && %player_pos_xf <= %brick_box[x2] && %player_pos_yf <= %brick_box[y2] && %player_pos_zf <= %brick_box[z2]) {
		if(%brick.liquidObj != -1) {
			talk("IN" SPC %brick SPC "$Mining::Brick[" @ %player_pos_xf @ "," @ %player_pos_yf @ "," @ %player_pos_zf @ "]" SPC %this.getPosition());
		} else {
			talk("NOT LIQUID" SPC "$Mining::Brick[" @ %player_pos_xf @ "," @ %player_pos_yf @ "," @ %player_pos_zf @ "]" SPC %this.getPosition());
		}
	}
}