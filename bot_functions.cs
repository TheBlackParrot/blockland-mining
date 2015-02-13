//shoutouts to Badspot for passwording setShapeName
//you made it hard for players to determine what type of bot they're fighting, I highly appreciate it. :)
//WONTFIX: name bots (you can't)

function AIPlayer::doDamage(%this,%client,%amount) {
	%this.health -= %amount;
	if(%this.health <= 0) {
		%this.health = 0;

		%client.exp += %this.maxHealth;
		%client.checkForLevelUp();

		%client.updateBottomPrint_Weapon();

		%this.kill();
	}
	%client.centerPrint(%this.health,1);
}

function AIPlayer::setMiningHealth(%this) {
	// for now
	// TODO: fire a radius search to get the biome, base the health scale off of that
	// TODO: add another scale, based on the player level from whoever "placed a mining brick"
	%this.health = %this.maxHealth = getRandom(50,70);
}

function GameConnection::checkForLevelUp(%this) {
	%level = %this.level;
	%exp = %this.exp;

	if(%exp >= getLevelCost(%level)) {
		%this.exp -= getLevelCost(%level);
		%this.level++;
		messageClient(%this,'',"\c6You are now \c3level" SPC %this.level @ "\c6! \c7(This is only a placeholder, the level system will get much more advanced later on)");
	}
}

function GameConnection::printBotInfo(%this,%bot) {
	%health = %bot.health;
	%max_health = %bot.maxHealth;
	%color = rgbToHex(%bot.headColor);
	%str[0] = "<just:left><font:Arial Bold:30><color:" @ %color @ ">" @ %bot.type @ "<just:right>\c6" @ %health @ "/" @ %max_health @ "HP";

	%str[1] = "<br><font:Courier:16><just:center>\c3-\c2";
	%pblen = 70;
	%width = mCeil((%health/%max_health)*%pblen);
	for(%i=0;%i<%width;%i++) {
		%add = %add @ "-";
	}
	%add = %add @ "\c0";
	for(%i=0;%i<(%pblen-%width);%i++) {
		%add = %add @ "-";
	}
	%str[1] = %str[1] @ %add @ "\c3-";

	%this.centerPrint(%str[0] @ %str[1],2);
}

function getLevelCost(%level) {
	return mPow(%level+15,2);
}

package MiningBotPackage {
	function ZombieHoleBot::onBotLoop(%this,%obj) {
		%obj.hFollowPlayer(%obj.hFindClosestPlayer());
		return parent::onBotLoop(%this,%obj);
	}
	function ZombieHoleBot::onAdd(%this,%obj) {
		%obj.setMaxForwardSpeed(3);
		%obj.setMaxSideSpeed(3);
		%obj.setMaxBackwardSpeed(3);

		%obj.setRandomAppearance();
		%obj.setMiningHealth();

		%obj.type = "Zombie";

		return parent::onAdd(%this,%obj);
	}

	//function fxDTSBrick::onBotDeath(%this) {
	//	%obj = %this.hBot;
	//	%this.schedule()
	//	return parent::onBotDeath(%this);
	//}

	function Player::removeBody(%this) {
		// this applies to both Player and AIPlayer??? what?
		if(%this.getClassName() $= "AIPlayer") {
			%brick = %this.spawnBrick;
			if(isObject(%brick)) {
				%brick.schedule(33,killBrick);
			}
		}
		return parent::removeBody(%this);
	}

	function SwordProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal) {
		if(%col.getClassName() $= "AIPlayer") {
			%col.doDamage(%obj.client,getRandom(5,10));
			%col.hFollowPlayer(%obj);
			%obj.client.printBotInfo(%col);
		}
	}

	function armor::onCollision(%this,%obj,%col,%fade,%pos,%normal) {
		if(%col.getClassName() $= "AIPlayer" && %obj.getClassName() $= "Player") {
			if(isObject(%obj)) {
				if(%obj.getState() !$= "Dead" && %col.getState() !$= "Dead") {
					%obj.doDamage(5,%col);
					%col.hSpazzClick();
				}
			}
		}
		return parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	}

	function serverCmdUseTool(%this,%id) {
		%this.updateBottomPrint_Weapon();
		return parent::serverCmdUseTool(%this,%id);
	}
};
activatePackage(MiningBotPackage);