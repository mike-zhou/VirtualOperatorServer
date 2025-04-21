
async function get(endpoint)
{
    response = await fetch(`/get/${endpoint}`);
    data = await response.text();
    return data;
}

async function post(endpoint, payload)
{
    response = await fetch(`/post/${endpoint}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    });
    data = await response.text();

    return data;
}

async function getVersion() {
    try {
        document.getElementById("id_versionResult").innerHTML = "";
        data = await get('Version');
        document.getElementById("id_versionResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function testEcho() {
    try {
        let echoCount = parseInt(document.getElementById("id_echoCount").value);
        if(isNaN(echoCount))
        {
            alert("Please enter a valid integer");
            return;
        }
        
        for(let i = 0; i < echoCount; i++)
        {
            let content = new Uint8Array(i%250);
            for(let j=0; j < content.byteLength; j++)
            {
                content[j] = j;
            }
            
            payload = 
            {
                data: Array.from(content)
            };
            
            data = await post('Echo', payload);
            if(data != "success")
            {
                alert(`ECHO failed at ${i}, reason: ${data}`);
                return;
            }

            document.getElementById("id_echoProgress").value = i * 100 / echoCount;
        }
        document.getElementById("id_echoProgress").value = 100;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function getGPIOMode()
{
    try {
        document.getElementById("id_gpioModeResult").innerHTML = "";
        data = await get('GPIOMode');
        document.getElementById("id_gpioModeResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function readGPIO() {
    try {
        document.getElementById("id_readGpioResult").innerHTML = "";
        data = await get('GPIO');
        document.getElementById("id_readGpioResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function setGpio(id)
{
    segments = id.split("_");
    if( (segments.length != 4) ||
        (segments[0] != 'id') ||
        (segments[1] != 'setGpio'))
    
    {
        console.error(`Error: wrong element id '${id}' for setGpio`);
        return;
    }
    
    const checkbox = document.getElementById(id);
    newValue = 0;
    if(checkbox.checked)
        newValue = 1;

    payload = {
        portName : segments[2],
        bitIndex : parseInt(segments[3], 10),
        level: newValue
    }

    data = await post('setGpio', payload);
    if(data != "success")
    {
        console.error(`Error: failed to toggle GPIO ${id}, info: ${data}`);
    }
}

async function onDocumentClick(event)
{
    const element = event.target;
    const targetWithId = element.closest('[id]');
    const elementId = targetWithId.id;

    if (elementId == "id_getVersion")
        await getVersion();
    else if (elementId == "id_testEcho")
        await testEcho();
    else if (elementId == 'id_getGpioMode')
        await getGPIOMode();
    else if (elementId == 'id_readGpio')
        await readGPIO();
    else if (elementId.startsWith("id_setGpio_"))
        await setGpio(elementId);
    else
        console.error(`Error: unknown element id: '${elementId}' in onDocumentClick()`);
}

async function checkPeripharalStatus()
{
    try {
        data = await get('PeripharalStatus')
        const status = JSON.parse(data)
        // Power
        let powerStatus = status.Power;
        for(let i=0; i<6; i++)
        {
            let po = `PO${i}`;
            let enabled = powerStatus[po]["Enabled"];
            let available = powerStatus[po]["Available"];

            let id = `id_powerOutput_set_${i}`;
            let powerSet = document.getElementById(id);
            powerSet.checked = enabled;
            
            id = `id_powerOutput_state_${i}`
            let powerState = document.getElementById(id);
            if(available)
            {
                powerState.className = "active-green-dot";
            }
            else
            {
                powerState.className = "inactive-green-dot";
            }
        }

        // Stepper Power
        powerStatus = status.StepperPower;
        for(let i=6; i<12; i++)
        {
            let po = `PO${i}`;
            let enabled = powerStatus[po]["Enabled"];
            let available = powerStatus[po]["Available"];

            let id = `id_powerOutput_set_${i}`;
            let powerSet = document.getElementById(id);
            powerSet.checked = enabled;
            
            id = `id_powerOutput_state_${i}`
            let powerState = document.getElementById(id);
            if(available)
            {
                powerState.className = "active-green-dot";
            }
            else
            {
                powerState.className = "inactive-green-dot";
            }
        }

        // BDC main power
        powerStatus = status.BDCPower;
        {
            let id = "id_bdcPowerMain_set";
            let powerSet = document.getElementById(id);
            powerSet.checked = powerStatus["Enabled"];

            id = "id_bdcPowerMain_state";
            let powerState = document.getElementById(id);
            if(powerStatus["Available"])
            {
                powerState.className = "active-green-dot";
            }
            else
            {
                powerState.className = "inactive-green-dot";
            }
        }

        // BDC motors
        let motorStatus = status.BDCMotor;
        for(let i=0; i<3; i++)
        {
            let bdc = motorStatus[`BDC${i}`];
            let control = bdc["Control"];
            let powerAvailable = bdc["PowerAvailable"];

            if(powerAvailable)
                document.getElementById(`id_bdcPowerOutput_state_${i}`).className = "active-green-dot";
            else
                document.getElementById(`id_bdcPowerOutput_state_${i}`).className = "inactive-green-dot";

            if(control == "Coast")
                document.getElementById(`id_bdc_coast_${i}`).checked = true;
            else if(control == "Forward")
                document.getElementById(`id_bdc_forward_${i}`).checked = true;
            else if(control == "Reverse")
                document.getElementById(`id_bdc_reverse_${i}`).checked = true;
            else if(control == "Brake")
                document.getElementById(`id_bdc_brake_${i}`).checked = true;
            else
                console.error(`Error: unknown BDC motor control '${control}'`);
        }

        // stepper motors
        motorStatus = status.StepperMotor;
        for(let i=0; i<10; i++)
        {
            let stepper = motorStatus[`stepper${i}`];
            let forward = stepper["forward"];
            let enable = stepper["enable"];
            let alarm = stepper["alarm"];

            if(alarm)
                document.getElementById(`id_stepperAlarm_state_${i}`).className = "active-red-dot";
            else
                document.getElementById(`id_stepperAlarm_state_${i}`).className = "inactive-red-dot";

            document.getElementById(`id_stepper_enable_${i}`).checked = enable;
            document.getElementById(`id_stepper_forward_${i}`).checked = forward;
        }

        // position detectors
        let pdStatus = status.PositionDetector;
        for(let i=0; i<20; i++)
        {
            let activated = pdStatus[`pd${i}`];
            if(activated)
                document.getElementById(`id_positionDetector_${i}`).className = "active-red-dot";
            else
                document.getElementById(`id_positionDetector_${i}`).className = "inactive-red-dot";
        }
        for(let i=20; i<49; i++)
        {
            let activated = pdStatus[`pd${i}`];
            if(activated)
                document.getElementById(`id_positionDetector_${i}`).className = "active-green-dot";
            else
                document.getElementById(`id_positionDetector_${i}`).className = "inactive-green-dot";
        }
        
    } catch (error) {
        console.error("Error:", error);
    }

    setTimeout(() => { checkPeripharalStatus(); }, 1000);
}

document.addEventListener('click', async function(event) {
    onDocumentClick(event);
} );

setTimeout(() => { checkPeripharalStatus(); }, 1000);

