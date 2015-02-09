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

	initContainerBoxSearch(%this.getPosition(), "0.1 0.1 0.1", $TypeMasks::FXBrickObjectType);
	while((%targetObject = containerSearchNext()) != 0 && isObject(%targetObject)) {
		if(%targetObject.liquidObj != -1) {
			if(%targetObject.hazardous) {
				%this.addHealth(-5);
			}
		}
	}
}