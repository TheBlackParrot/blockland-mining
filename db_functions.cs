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