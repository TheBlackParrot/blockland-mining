//for(%i=1;%i<=28;%i++) { echo(%i SPC mpow(%i,1.75)); }

function GameConnection::setCosts(%this,%which) {
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	%scales = "1.75 1.65 1.5 1.4 1.25 1 0.95 0.9 0.85 0.8 0.75 0.7 0.65 0.6 0.55";

	if(!%this.level[%which]) {
		%this.level[%which] = 1;
	}
	%level = %this.level[%which];

	for(%i=0;%i<getWordCount(%ores);%i++) {
		%ore = getWord(%ores,%i);
		%scale = getWord(%scales,%i);
		%cost = mpow(%level+3,%scale);
		if(%cost < 11.3 || %cost > 50 && %i <= 10) {
			continue;
		} else {
			%level_str = %level_str @ %ore SPC mFloor(%cost) @ "\t";
		}
	}
	%this.costs[%which] = %level_str;
}

function GameConnection::increaseLevel(%this,%which) {
	if(!%this.costs[%which]) {
		%this.costs[%which] = %this.setCosts(%which);
	}
	%costs = %this.costs[%which];

	%can_increase = 1;

	if(%this.getMiningDelay() <= 100 && %which $= "speed") {
		messageClient(%this,'',"\c6You cannot increase your \c3speed level \c6any further.");
		return;
	}

	for(%i=0;%i<getFieldCount(%costs);%i++) {
		%field = getField(%costs,%i);

		if(%field !$= "") {
			%ore = getWord(%field,0);
			%cost = getWord(%field,1);

			if(%this.amount[%ore] < %cost) {
				%can_increase = 0;
				messageClient(%this,'',"\c6You need <color:" @ getOreColor(%ore) @ ">" @ %cost - %this.amount[%ore] SPC "more" SPC %ore SPC "\c6to increase your\c3" SPC %which SPC "level \c6to \c3level" SPC %this.level[%which] + 1 @ "\c6.");
			}
		}
	}

	if(%can_increase) {
		%this.level[%which]++;
		for(%i=0;%i<getFieldCount(%costs);%i++) {
			%field = getField(%costs,%i);

			if(%field !$= "") {
				%ore = getWord(%field,0);
				%cost = getWord(%field,1);
				%this.amount[%ore] -= %cost;
			}
		}
		//messageClient(%this,'',"\c6Your\c3" SPC %which SPC "level \c6has been increased to \c4level" SPC %this.level[%which] @ "\c6!");
		messageAll('',"\c3" @ %this.name SPC "\c6has just increased their\c3" SPC %which SPC "level \c6to \c4level" SPC %this.level[%which] @ "\c6!");
		serverPlay2D(level_up);
		serverCmdMiningServer_requestGUIVars(%this);
	} else {
		%this.play2D(errorSound);
	}
}