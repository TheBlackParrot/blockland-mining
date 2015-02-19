function Player::miningLoop(%this) {
	if(%this.miningLoop) {
		cancel(%this.miningLoop);
	}
	if(%this.currTool != -1 || %this.getState() $= "Dead") {
		%this.stopMining();
		return;
	}
	%this.lastTriggered = getSimTime();

	%brick = %this.getLookingAt();
	if(isObject(%brick)) {
		if(%brick.type $= "Natural Gas") {
			return;
		}
		%brick.mineBrick(%this);
		%brick.setLight("Mining_LightTrigger");
		%brick.lightSched = %brick.schedule(70,setLight,Mining_Light @ %brick.colorID);
		%brick.setColorFX(3);
		%this.playThread(1,activate);
	}

	if(%this.wasLookingAt != %brick && isObject(%this.wasLookingAt)) {
		%this.wasLookingAt.setLight(0);
		%this.wasLookingAt.setColorFX(0);
	}
	%this.wasLookingAt = %brick;
	%this.miningLoop = %this.schedule(%this.client.getMiningDelay(),miningLoop);

	// reducing data sent
	if(%this.oldPos !$= %this.getPosition()) {
		%this.client.sendGUIVar("position",%this.getPosition());
	}
	%this.oldPos = %this.getPosition();
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
	if(%this.health <= 0 && %this.getState() !$= "Dead" && !%this.isDead) {
		%this.isDead = 1;
		%this.health = 0;

		if(isObject(%killer)) {
			messageAll('',%this.client.name SPC "was killed by a" SPC %killer.type);
		} else {
			messageAll('',%this.client.name SPC "died from" SPC %killer);
		}
		%this.kill();
	}

	// apparently i need this check. why
	if(isObject(%this.client)) {
		%this.client.sendGUIVar("health",%this.health,%this.client.maxHealth);
		%this.client.updateBottomPrint_Weapon();
	}
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
	%this.bottomPrint(%str[1] @ %str[2],5);
}

function GameConnection::getMiningDelay(%this) {
	//increase from 8 to 15 should now max out at level 20 instead of 37
	%speed = 400 - ((%this.level[speed]-1) * 15);
	if(%speed < 100) {
		%speed = 100;
	}
	return %speed;
}

function GameConnection::doExplosionSound(%this,%target) {
	if(!isObject(%target)) {
		return;
	}

	%targetPlayer = %target.player;
	%player = %this.player;
	if(!isObject(%player)) {
		%player = %this.Camera;
	}
	if(!isObject(%targetPlayer)) {
		%targetPlayer = %target.Camera;
	}

	%dist = vectorDist(%player.getPosition(),%targetPlayer.getPosition());
	if(%dist < 40) {
		%this.play2D(explosion_close);
	}
	if(%dist >= 40 && %dist < 200) {
		%this.play2D(explosion_neither);
	}
	if(%dist >= 200) {
		%this.play2D(explosion_far);
	}
}

package MiningPlayerPackage {
	function armor::onTrigger(%db,%obj,%slot,%val) {
		if(%obj.getClassName() $= "Player" && !%slot) {
			if(%val == 1) {
				if(%obj.currTool == -1 || %obj.currTool $= "") {
					if(getSimTime() - %obj.lastTriggered > %obj.client.getMiningDelay()) {
						%obj.miningLoop();
					}
				}
			} else {
				%obj.stopMining();
			}
		}
		if(%obj.getClassName() $= "Player" && %slot == 4 && %val) {
			initContainerBoxSearch(%obj.getPosition(), "8 8 8", $TypeMasks::FXBrickObjectType);
			while((%targetObject = containerSearchNext()) != 0 && isObject(%targetObject)) {
				if(%targetObject.type $= "Natural Gas") {
					if(!%obj.isDead) {
						%targetObject.mineBrick(%obj);
					}
				}
			}
		}

		return Parent::onTrigger(%db,%obj,%slot,%val);
	}

	function GameConnection::spawnPlayer(%this) {
		// will expand later
		parent::spawnPlayer(%this);
		%this.player.health = %this.maxHealth = 100;
		%this.player.currTool = -1;
		serverCmdMiningServer_requestGUIVars(%this);
	}
};
activatePackage(MiningPlayerPackage);