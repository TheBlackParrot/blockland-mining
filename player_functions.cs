function Player::miningLoop(%this) {
	if(%this.miningLoop) {
		cancel(%this.miningLoop);
	}
	if(%this.currTool != -1) {
		return;
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
	%this.miningLoop = %this.schedule(%this.client.getMiningDelay(),miningLoop);
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

function Player::doDamage(%this,%amount,%killer) {
	%this.health -= %amount;
	%this.setDamageLevel(0);
	%this.setDamageFlash(%amount/(%this.client.maxHealth/5));
	if(%this.health <= 0) {
		%this.health = 0;
		if(isObject(%killer)) {
			messageAll('',%this.client.name SPC "was killed by a" SPC %killer.type);
		} else {
			messageAll('',%this.client.name SPC "died from" SPC %killer);
		}
		%this.kill();
	}
	%this.client.updateBottomPrint_Weapon();
}

function Player::increaseHealth(%this,%amount) {
	%this.health += %amount;
	if(%this.health >= %this.client.maxHealth) {
		%this.health = %this.client.maxHealth;
	}
	%this.setWhiteOut(0.1);
}

function GameConnection::updateBottomPrint(%this) {
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	for(%i=0;%i<getWordCount(%ores);%i++) {
		%ore = getWord(%ores,%i);
		if(%this.amount[%ore]) {
			%amount_str = %amount_str @ "<color:" @ getOreColor(%ore) @ ">" @ strUpr(getSubStr(%ore,0,1)) @ "\c6" @ %this.amount[%ore] @ " ";
		}
	}
	%this.bottomPrint(%amount_str);
}
function GameConnection::updateBottomPrint_Weapon(%this) {
	%str[1] = "<color:ffaaaa>" @ %this.player.health @ "/" @ %this.maxHealth SPC "HP";
	%str[2] = "<just:right>\c3LV" SPC %this.level SPC "\c5" @ %this.exp @ "/" @ getLevelCost(%this.level) SPC "EXP";
	%this.bottomPrint(%str[1] @ %str[2]);
}

function GameConnection::getMiningDelay(%this) {
	%speed = 400 - ((%this.level[speed]-1) * 8);
	if(%speed < 100) {
		%speed = 100;
	}
	return %speed;
}

package MiningPlayerPackage {
	function armor::onTrigger(%db,%obj,%slot,%val) {
		if(%obj.getClassName() $= "Player" && !%slot) {
			if(%val == 1) {
				if(%obj.currTool == -1 || %obj.currTool $= "") {
					if(getSimTime() - %obj.lastTriggered > %obj.client.getMiningDelay()) {
						%obj.miningLoop();
						%obj.lastTriggered = getSimTime();
					}
				}
			} else {
				%obj.stopMining();
			}
		}

		return Parent::onTrigger(%db,%obj,%slot,%val);
	}

	function GameConnection::spawnPlayer(%this) {
		// will expand later
		parent::spawnPlayer(%this);
		%this.player.health = %this.maxHealth = 100;
		%this.player.currTool = -1;
	}
};
activatePackage(MiningPlayerPackage);