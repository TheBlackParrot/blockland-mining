function Mining_doExplosionStep(%pos,%radius,%o_realtime) {
	InitContainerRadiusSearch(%pos,%radius,$TypeMasks::FXBrickObjectType);
	while((%targetObject = containerSearchNext()) != 0) {
		if(!%targetObject.permanent) {
			%targetObject.placeSurroundings();
			if(%targetObject)
				%targetObject.delete();
		}
	}
	if(%radius != 0)
		schedule((getRealTime() - %o_realtime),0,Mining_doExplosionStep,%pos,%radius-1,getRealTime());
}

function Mining_doExplosion(%pos,%radius) {
	Mining_doExplosionStep(%pos,%radius+5,getRealTime());
}

function serverCmdExplosionTest(%client,%radius) {
	%this = %client.player;
	%eye = vectorScale(%this.getEyeVector(), 8);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FxBrickObjectType;
	%hit = firstWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this));

	if(!isObject(%hit))
		return;
	if(%hit.getClassName() $= "fxDTSBrick" && !%hit.permanent) {
		talk(%hit.getPosition());
		Mining_doExplosion(%hit.getPosition(),%radius);
	}
}