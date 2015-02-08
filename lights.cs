function Mining_initLights() {
	for(%i=0;%i<64;%i++) {
		if(getColorIDTable(%i) !$= "") {
			datablock fxLightData(DummyLightName)
			{
				uiName = "ColorSetLight_" @ %i;

				LightOn = true;
				radius = 9;
				brightness = 4;
				color = getColorIDTable(%i);

				flareOn = false;
			};
			DummyLightName.setName("Mining_Light" @ %i);
		} else {
			echo("Stopped at" SPC %i);
			break;
		}
	}
}

datablock fxLightData(Mining_LightTrigger)
{
	uiName = "ColorSetLight_Trigger";

	LightOn = true;
	radius = 15;
	brightness = 4;
	color = "1 1 1 1";

	flareOn = false;
};

PlayerLight.radius = 8;
PlayerLight.brightness = 4;