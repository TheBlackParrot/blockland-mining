//for(%i=1;%i<=28;%i++) { echo(%i SPC mpow(%i,1.75)); }

function GameConnection::setCosts(%this,%which) {
	%ores = "copper coal silver iron gold platinum titanium diamond uranium plutonium solarium aegisalt rubium violium erchius";
	%scales = "1.75 1.65 1.5 1.4 1.25 1 0.95 0.9 0.85 0.8 0.75 0.7 0.65 0.6 0.55";

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

function GameConnection::getCosts(%this,%which) {
	
}

package MiningLevelPackage {
	function GameConnection::autoAdminCheck(%this) {
		%this.level[power] = 1;
		%this.level[speed] = 1;
		%this.setCosts();

		return parent::autoAdminCheck(%this);
	}
};