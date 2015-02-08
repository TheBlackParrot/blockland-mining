function getBiome(%brick) {
	if(!%brick || !isObject(%brick) || %brick == -1) {
		return BiomeList.getObject(1);
	}

	for(%i=0;%i<BiomeList.getCount();%i++) {
		%biome[%i] = BiomeList.getObject(%i);
		%chance_total += %biome[%i].rarity;
		%biome[%i,min_range] = %chance_total - %biome[%i].rarity;
		%biome[%i,max_range] = %chance_total;
	}

	%rand = getRandom(0,%chance_total);
	if(getRandom(0,100) <= 99) {
		return %brick.biomeObj;
	} else {
		for(%i=0;%i<BiomeList.getCount();%i++) {
			if(%rand >= %biome[%i,min_range] && %rand <= %biome[%i,max_range]) {
				return %biome[%i];
			}
		}
	}

	return 1;
}

function getOre(%brick) {
	%ore_count = 0;
	for(%i=0;%i<OreList.getCount();%i++) {
		%row = OreList.getObject(%i);
		if(stripos(%row.lvl,%brick.biomeObj.max_ore_lvl) != -1) {
			%ore[%ore_count] = OreList.getObject(%i);
			%chance_total += %ore[%ore_count].rarity;
			%ore[%ore_count,min_range] = %chance_total - %ore[%ore_count].rarity;
			%ore[%ore_count,max_range] = %chance_total;
			%ore_count++;
		}
	}

	%rand = getRandom(0,%chance_total);
	if(%brick.oreObj != -1 && %brick.oreObj !$= "") {
		return %brick.oreObj;
	} else {
		for(%i=0;%i<%ore_count;%i++) {
			if(%rand >= %ore[%i,min_range] && %rand <= %ore[%i,max_range]) {
				talk("OBTAINED" SPC %ore[%i].type);
				return %ore[%i];
			}
		}
	}

	return OreList.getObject(1);
}