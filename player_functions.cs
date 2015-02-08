function Player::miningLoop(%this) {
	if(%this.miningLoop) {
		cancel(%this.miningLoop);
	}

	%brick = %this.getLookingAt();
	if(isObject(%brick)) {
		%brick.mineBrick(%this);
		%brick.setLight("Mining_LightTrigger");
		%brick.lightSched = %brick.schedule(70,setLight,Mining_Light @ %brick.colorID);
		%brick.setColorFX(3);
	}

	if(%this.wasLookingAt != %brick && isObject(%this.wasLookingAt)) {
		%this.wasLookingAt.setLight(0);
		%this.wasLookingAt.setColorFX(0);
	}
	%this.wasLookingAt = %brick;
	%this.miningLoop = %this.schedule(150,miningLoop);
}
function Player::stopMining(%this) {
	cancel(%this.miningLoop);
	if(isObject(%this.wasLookingAt)) {
		%brick = %this.wasLookingAt;
		%brick.setLight(0);
		%brick.setColorFX(0);
		cancel(%brick.lightSched);
	}
}

package MiningPlayerPackage {
	function armor::onTrigger(%db,%obj,%slot,%val) {
		if(%obj.getClassName() $= "Player" && !%slot) {
			if(%val == 1) {
				if(getSimTime() - %obj.lastTriggered > 500) {
					%obj.miningLoop();
					%obj.lastTriggered = getSimTime();
				}
			} else {
				%obj.stopMining();
			}
		}

		return Parent::onTrigger(%db,%obj,%slot,%val);
	}
};
activatePackage(MiningPlayerPackage);

function Player::getLookingAt(%this,%distance)
{
	if(!%distance) {
		%distance = 15;
	}

	%eye = vectorScale(%this.getEyeVector(),%distance);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FxBrickObjectType;
	%hit = firstWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this));
		
	if(!isObject(%hit)) {
		return;
	}
		
	if(%hit.getClassName() $= "fxDTSBrick") {
		return %hit;
	}
}

function GameConnection::updateBottomPrint(%this) {
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	for(%i=0;%i<getWordCount(%ores);%i++) {
		%ore = getWord(%ores,%i);
		for(%j=0;%j<OreList.getCount();%j++) {
			%row = OreList.getObject(%j);
			if(strLwr(%row.type) $= %ore) {
				break;
			}
		}
		if(%this.amount[%ore]) {
			%amount_str = %amount_str @ "<color:" @ RGBToHex(getColorIDTable(%row.color)) @ ">" @ strUpr(getSubStr(%ore,0,1)) @ "\c6" @ %this.amount[%ore] @ " ";
		}
	}
	%this.bottomPrint(%amount_str);
}