
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

    payload = [
        {
            PortName : segments[2],
            BitIndex : parseInt(segments[3], 10),
            Level: newValue
        }
    ];

    data = await post('setGpio', payload);
    if(data != "success")
    {
        console.error(`Error: failed to toggle GPIO ${id}, info: ${data}`);
    }
}

async function setPowerOutput(id) 
{
    segments = id.split('_');

    powerNumber = segments[segments.length - 1];
    enablePower = document.getElementById(id).checked;

    payload = 
        { 
            powerOutput: parseInt(powerNumber, 10),
            enable: enablePower
        };
        
    data = await post('setPowerOutput', payload);
    if(data != "success")
    {
        console.error(`Error: failed to set PO ${powerNumber}, info: ${data}`);
    }
}

async function setBDCPowerOutput(id) 
{
    enablePower = document.getElementById(id).checked;

    payload = 
        { 
            enable: enablePower
        };
        
    data = await post('setBDCPowerOutput', payload);
    if(data != "success")
    {
        console.error(`Error: failed to set PO ${powerNumber}, info: ${data}`);
    }
}

async function setBDCControl(id) 
{
    segments = id.split("_");
    actionStr = segments[2];
    indexStr = segments[3];

    payload = 
        { 
            action: actionStr,
            index: parseInt(indexStr, 10)
        };
        
    data = await post('setBDCControl', payload);
    if(data != "success")
    {
        console.error(`Error: failed to set PO ${powerNumber}, info: ${data}`);
    }
}

async function setStepper(id) 
{
    segments = id.split("_");
    action = segments[2];

    if(action == "disable")
    {
        payload = {
            index: parseInt(segments[3], 10),
            disableStepper: document.getElementById(id).checked
        }

        data = await post('disableStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to enabl stepper ${segments[3]}, info: ${data}`);
        }
    }
    else if(action == "forward")
    {
        payload = {
            index: parseInt(segments[3], 10),
            forwardStepper: document.getElementById(id).checked
        }

        data = await post('forwardStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to forward stepper ${segments[3]}, info: ${data}`);
        }
    }
    else if(action == "clock")
    {
        payload = {
            index: parseInt(segments[3], 10),
            highLevel: document.getElementById(id).checked
        }

        data = await post('clockStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to clock stepper ${segments[3]}, info: ${data}`);
        }
    }
    else if(action == "go")
    {
        stepsNum = NaN;

        steps = segments[3];
        if(steps == "steps")
        {
            stepsNum = document.getElementById(`id_stepper_steps_${segments[4]}`).valueAsNumber;
            if(Number.isNaN(stepsNum))
            {
                alert(`Error: No valid number in id_stepper_steps_${segments[4]}`);
                return;
            }
        }
        else
        {
            stepsNum = parseInt(steps, 10);
        }

        if((stepsNum < 0) || (stepsNum > 1024))
        {
            alert(`Error: Out of range of clocks ${stepsNum}`);
            return;
        }

        payload = {
            index: parseInt(segments[4], 10),
            clocks: stepsNum
        }

        data = await post('runStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to run stepper ${segments[4]}, info: ${data}`);
        }
    }
    else if(action == "steps")
    {
        // do nothing when id_stepper_steps_X is clicked
    }
    else
    {
        alert(`Unsupported action '{action}' in '{id}'`);
    }
}

async function onDocumentClick(event)
{
    const element = event.target;
    elementId = element.id;
    if(elementId == "")
    {
        if(element.htmlFor)
        {
            elementId = element.htmlFor;
        }
    }

    if(elementId == "")
        return;

    if (elementId == "id_getVersion")
        await getVersion();
    else if (elementId == "id_testEcho")
        await testEcho();
    else if (elementId.startsWith("id_setGpio_"))
        await setGpio(elementId);
    else if (elementId.startsWith("id_powerOutput_set_"))
        await setPowerOutput(elementId);
    else if (elementId == "id_bdcPowerMain_set")
        await setBDCPowerOutput(elementId);
    else if (elementId.startsWith("id_bdcControl_"))
        await setBDCControl(elementId);
    else if (elementId.startsWith("id_stepper_"))
        await setStepper(elementId);
    else
        alert(`Error: unknown element id: '${elementId}' in onDocumentClick()`);

    // refresh webpage
    await post('refreshStatus', {});
    await refreshData();
}

async function checkPeripharalStatus()
{
    try {
        const data = await get('PeripharalStatus');
        const status = JSON.parse(data);

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
                document.getElementById(`id_bdcControl_coast_${i}`).checked = true;
            else if(control == "Forward")
                document.getElementById(`id_bdcControl_forward_${i}`).checked = true;
            else if(control == "Reverse")
                document.getElementById(`id_bdcControl_reverse_${i}`).checked = true;
            else if(control == "Brake")
                document.getElementById(`id_bdcControl_brake_${i}`).checked = true;
            else
                console.error(`Error: unknown BDC motor control '${control}'`);
        }

        // stepper motors
        motorStatus = status.StepperMotor;
        for(let i=0; i<10; i++)
        {
            let stepper = motorStatus[`stepper${i}`];
            let forward = stepper["forward"];
            let disable = stepper["disable"];
            let alarm = stepper["alarm"];
            let clock = stepper["clock"];

            if(alarm)
                document.getElementById(`id_stepperAlarm_state_${i}`).className = "active-red-dot";
            else
                document.getElementById(`id_stepperAlarm_state_${i}`).className = "inactive-red-dot";

            document.getElementById(`id_stepper_disable_${i}`).checked = disable;
            document.getElementById(`id_stepper_forward_${i}`).checked = forward;
            document.getElementById(`id_stepper_clock_${i}`).checked = clock;
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
        console.error("Error in checkPeripharalStatus():", error);
    }
}

async function checkEncoders()
{
    try 
    {
        const data = await get('Encoders');
        const status = JSON.parse(data);

        const lptim1Counter = status.lptim1Counter;
        const lptim2Counter = status.lptim2Counter;
        const htim1Counter = status.htim1Counter;
        const htim2Counter = status.htim2Counter;
        const htim3Counter = status.htim3Counter;
        const htim4Counter = status.htim4Counter;
        const htim5Counter = status.htim5Counter;
        const htim8Counter = status.htim8Counter;

        document.getElementById("id_encoder_0").textContent = lptim1Counter.toString();
        document.getElementById("id_encoder_1").textContent = lptim2Counter.toString();
        document.getElementById("id_encoder_2").textContent = htim1Counter.toString();
        document.getElementById("id_encoder_3").textContent = htim2Counter.toString();
        document.getElementById("id_encoder_4").textContent = htim3Counter.toString();
        document.getElementById("id_encoder_5").textContent = htim4Counter.toString();
        document.getElementById("id_encoder_6").textContent = htim5Counter.toString();
        document.getElementById("id_encoder_7").textContent = htim8Counter.toString();
    } 
    catch (error) {
        console.error("Error in checkEncoders():", error);
    }
}

async function updateStatus() 
{
    try {
        data = await get('DynamicStatus');
        document.getElementById("id_dynamic_status").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
    
}

async function refreshData()
{
    await readGPIO();
    await updateStatus();
    await checkPeripharalStatus();
    await checkEncoders();
}

document.addEventListener('click', async function(event) { onDocumentClick(event); } );

let intervalId = setInterval(refreshData, 1000);
