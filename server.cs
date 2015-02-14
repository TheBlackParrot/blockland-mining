// delaying execution of light creating until the colorset can be loaded in its entirety
exec("./lights.cs");
schedule(0,0,Mining_initLights);
exec("./sounds.cs");

exec("./support.cs");

//ugh, blockland
$Mining::Root = "Add-Ons/Gamemode_Mining2";

function setMiningVersion() {
	if(!isFile($Mining::Root @ "/.git/refs/heads/master")) {
		$Mining::Version = "UNDF?";
		return;
	}

	%file = new FileObject();
	%file.openForRead($Mining::Root @ "/.git/refs/heads/master");

	$Mining::Version = getSubStr(%file.readLine(),0,7);

	%file.close();
	%file.delete();
}
setMiningVersion();

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

	%liquids = new SimSet(LiquidList);
	%file.openForRead($Mining::Root @ "/db/liquids.db");
	while(!%file.isEOF()) {
		%line = %file.readLine();
		if(getSubStr(%line,0,2) $= "//") {
			continue;
		}
		%liquid = new ScriptObject(MiningBiome) {
			type = getField(%line,0);
			color = getField(%line,1);
			rarity = getField(%line,2);
			hazardous = getField(%line,3);
		};
		%liquids.add(%liquid);
	}
	%file.close();

	%file.delete();

	%set.add(%biomes);
	%set.add(%ores);
	%set.add(%liquids);

	return %set;
}
if(!isObject($MiningSet)) {
	$MiningSet = initMining();
}

exec("./player_functions.cs");
exec("./brick_functions.cs");
exec("./liquid_functions.cs");
exec("./db_functions.cs");
exec("./bot_functions.cs");
exec("./explosions.cs");
exec("./levels.cs");
exec("./commands.cs");
exec("./saving.cs");

PlayerStandardArmor.jumpForce = "1300";

package MiningServerPackage {
	function onServerDestroyed() {
		deleteVariables("$Mining::*");
		return parent::onServerDestroyed();
	}
};
activatePackage(MiningServerPackage);

function doSpawn() {
	Mining_newBrick(48,48,2500);
	schedule(400,0,Mining_doExplosion,$Mining::Brick[48,48,2500],30);

	PlayerDropPoints.delete();
	%points = new SimGroup(PlayerDropPoints) {
		new SpawnSphere() {
			position = "48 48 2350";
			rotation = "0 0 1 130.062";
			scale = "0.940827 1.97505 1";
			dataBlock = "SpawnSphereMarker";
			canSetIFLs = "0";
			radius = "20";
			sphereWeight = "1";
			indoorWeight = "1";
			outdoorWeight = "1";
			RayHeight = "150";
		};
	};
	MissionGroup.add(%points);
}
schedule(100,0,doSpawn);