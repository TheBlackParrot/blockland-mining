// delaying execution of light creating until the colorset can be loaded in its entirety
exec("./lights.cs");
schedule(0,0,Mining_initLights);
exec("./sounds.cs");

function initMining() {
	%set = new SimSet(MiningClass) {
		timeInit = getSimTime();
	};
	return %set;
}
if(!isObject($MiningClass)) {
	$MiningClass = initMining();
}

exec("./player_functions.cs");
exec("./brick_functions.cs");

package MiningServerPackage {
	function onServerDestroyed() {
		deleteVariables("$Mining::*");
		return parent::onServerDestroyed();
	}
};
activatePackage(MiningServerPackage);