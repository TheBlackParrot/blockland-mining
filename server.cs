// delaying execution of light creating until the colorset can be loaded in its entirety
exec("./lights.cs");
schedule(0,0,Mining_initLights);
exec("./sounds.cs");

exec("./support.cs");

//ugh, blockland
$Mining::Root = "Add-Ons/Gamemode_Mining2";

function initMining() {
	if(isObject(BiomeList)) {
		for(%i=0;%i<BiomeList.getCount();%i++) {
			BiomeList.getObject(%i).delete();
		}
		BiomeList.delete();
	}
	if(isObject(MiningClass)) {
		for(%i=0;%i<MiningClass.getCount();%i++) {
			MiningClass.getObject(%i).delete();
		}
		MiningClass.delete();
	}

	%set = new SimSet(MiningClass) {
		timeInit = getSimTime();
	};

	%file = new FileObject();

	%biomes = new SimSet(BiomeList);
	%file.openForRead($Mining::Root @ "/db/biomes.db");
	while(!%file.isEOF()) {
		%line = %file.readLine();
		if(getSubStr(%line,0,2) $= "//") {
			continue;
		}
		%biome = new ScriptObject(MiningBiome) {
			type = getField(%line,0);
			color = getField(%line,1);
			health = getField(%line,2);
			rarity = getField(%line,3);
			max_ore_lvl = getField(%line,4);
		};
		%biomes.add(%biome);
	}
	%file.close();

	%ores = new SimSet(OreList);
	%file.openForRead($Mining::Root @ "/db/ores.db");
	while(!%file.isEOF()) {
		%line = %file.readLine();
		if(getSubStr(%line,0,2) $= "//") {
			continue;
		}
		%ore = new ScriptObject(MiningBiome) {
			type = getField(%line,0);
			color = getField(%line,1);
			health = getField(%line,2);
			rarity = getField(%line,3);
			value = getField(%line,4);
			lvl = getField(%line,5);
		};
		%ores.add(%ore);
	}
	%file.close();

	%file.delete();

	%set.add(%biomes);
	%set.add(%ores);

	return %set;
}
if(!isObject($MiningSet)) {
	$MiningSet = initMining();
}

exec("./player_functions.cs");
exec("./brick_functions.cs");
exec("./db_functions.cs");
exec("./explosions.cs");

package MiningServerPackage {
	function onServerDestroyed() {
		deleteVariables("$Mining::*");
		return parent::onServerDestroyed();
	}
};
activatePackage(MiningServerPackage);