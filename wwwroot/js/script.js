
function createGpioTable()
{
    let portNameList = [
        "PA",
        "PB",
        "PC",
        "PD",
        "PE",
        "PF",
        "PG",
        "PH",
        "PI",
        "PJ",
        "PK"
    ];

    let html = [];

    html.push("<div>");
    html.push("<h1>GPIO</h1>")
    html.push("<table>");
    for(let portIndex = 0; portIndex < portNameList.length; portIndex++)
    {
        let portName = portNameList[portIndex];
        html.push("<tr>");
        for(let bitIndex = 0; bitIndex < 16; bitIndex++)
        {
            let labelId = `id_setGpio_${portName}_${bitIndex}_label`;
            let inputId = `id_setGpio_${portName}_${bitIndex}`;
            
            html.push("<td>");
            html.push(`<label id="${labelId}" for="${inputId}"> ${portName}${bitIndex}: </label>`);
            html.push(`<input type="checkbox" id="${inputId}">`)
            html.push("</td>");
        }
        html.push("</tr>");
    }
    html.push("</table>");
    html.push("</div>");

    return html.join("");
}

function createPowerTable()
{
    let html = [];

    html.push("<div>");
    html.push("<h1>Power Switch</h1>");
    html.push("<table>");
    html.push("<tr>");
    
    html.push("<td>");
    html.push("<div>");
    html.push("<h2>OPT</h2>");
    for(let i=0; i<6; i++)
    {
        html.push("<div>");
        html.push(`<label>PO ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_powerOutput_state_${i}"></div>`);
        html.push(`<label id="id_powerOutput_set_${i}_label" for="id_powerOutput_set_${i}"> Enable </label>`);
        html.push(`<input type="checkbox" id="id_powerOutput_set_${i}">`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    html.push("<td>");
    html.push("<div>");
    html.push("<h2>Motor</h2>");
    for(let i=6; i<12; i++)
    {
        html.push("<div>");
        html.push(`<label>PO ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_powerOutput_state_${i}"></div>`);
        html.push(`<label id="id_powerOutput_set_${i}_label" for="id_powerOutput_set_${i}"> Enable </label>`);
        html.push(`<input type="checkbox" id="id_powerOutput_set_${i}">`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    html.push("<td>");
    html.push("<div>");
    html.push("<h2>BDC</h2>");
    {
        html.push("<div>");
        html.push(`<label>BDC Power: </label>`);
        html.push(`<div class="unknown-dot" id="id_bdcPowerMain_state"></div>`);
        html.push(`<label id="id_bdcPowerMain_set_label" for="id_bdcPowerMain_set"> Enable </label>`);
        html.push(`<input type="checkbox" id="id_bdcPowerMain_set">`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    html.push("</tr>");
    html.push("</table>");
    html.push("</div>");

    return html.join("");
}

function createPositionDetectorTable()
{
    let html = [];

    html.push("<div>");
    html.push("<h1>Position Detector</h1>");
    html.push("<table>");
    html.push("<tr>");

    // column 0
    html.push("<td>");
    html.push("<div>");
    for(let i=0; i<10; i++)
    {
        html.push("<div>");
        html.push(`<label>PD ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_positionDetector_${i}"></div>`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    // column 1
    html.push("<td>");
    html.push("<div>");
    for(let i=10; i<20; i++)
    {
        html.push("<div>");
        html.push(`<label>PD ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_positionDetector_${i}"></div>`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    // column 2
    html.push("<td>");
    html.push("<div>");
    for(let i=20; i<30; i++)
    {
        html.push("<div>");
        html.push(`<label>PD ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_positionDetector_${i}"></div>`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    // column 3
    html.push("<td>");
    html.push("<div>");
    for(let i=30; i<40; i++)
    {
        html.push("<div>");
        html.push(`<label>PD ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_positionDetector_${i}"></div>`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    // column 4
    html.push("<td>");
    html.push("<div>");
    for(let i=40; i<49; i++)
    {
        html.push("<div>");
        html.push(`<label>PD ${i}: </label>`);
        html.push(`<div class="unknown-dot" id="id_positionDetector_${i}"></div>`);
        html.push("</div>");
    }
    html.push("</div>");
    html.push("</td>");

    html.push("</tr>");
    html.push("</table>");
    html.push("</div>");

    return html.join("");
}

function createBdcControlTable()
{
    let html = [];

    html.push("<div>");
    html.push("<h1>BDC Control</h1>");
    html.push("<table>");
    html.push("<tr>");

    for(let i=0; i<3; i++)
    {
        html.push("<td>");
        html.push("<div>");
        
        html.push(`<h2>PO${12 + i} BDC ${i}</h2>`);

        html.push("<div>");
        html.push(`<label>Power:</label>`);
        html.push(`<div class="unknown-dot" id="id_bdcPowerOutput_state_${i}"></div>`);
        html.push("</div>");

        html.push("<div>");
        html.push(`<label id="id_bdcControl_coast_${i}_label" for="id_bdcControl_coast_${i}">Coast</label>`);
        html.push(`<input type="radio" id="id_bdcControl_coast_${i}" name="bdc_${i}">`);
        html.push("</div>");

        html.push("<div>");
        html.push(`<label id="id_bdcControl_reverse_${i}_label" for="id_bdcControl_reverse_${i}">Reverse</label>`);
        html.push(`<input type="radio" id="id_bdcControl_reverse_${i}" name="bdc_${i}">`);
        html.push("</div>");

        html.push("<div>");
        html.push(`<label id="id_bdcControl_forward_${i}_label" for="id_bdcControl_forward_${i}">Forward</label>`);
        html.push(`<input type="radio" id="id_bdcControl_forward_${i}" name="bdc_${i}">`);
        html.push("</div>");

        html.push("<div>");
        html.push(`<label id="id_bdcControl_brake_${i}_label" for="id_bdcControl_brake_${i}">Brake</label>`);
        html.push(`<input type="radio" id="id_bdcControl_brake_${i}" name="bdc_${i}">`);
        html.push("</div>");

        html.push("</div>");
        html.push("</td>");
    }

    html.push("</tr>");
    html.push("</table>");
    html.push("</div>");

    return html.join("");
}

function createDynamicStatus()
{
    let html = [];

    html.push("<div>");
    html.push("<h1>Dynamic Status</h1>");
    html.push("<table>");
    html.push(`<tr><td>Main loop/s: </td><td><label id="id_dynamicStatus_mainLoop" /></td></tr>`);
    html.push(`<tr><td>Max flex timer ISR period: </td><td><label id="id_dynamicStatus_flexTimerIsrPeriod" /></td></tr>`);
    html.push(`<tr><td>Max fix timer ISR period: </td><td><label id="id_dynamicStatus_fixTimerIsrPeriod" /></td></tr>`);
    html.push("</table>");
    html.push("</div>");

    return html.join("");
}

function createEncoderTable()
{
    let html = [];

    let encoderAttributesList = [
        ["ENC0", "TIM2",    "id_encoder_0"],
        ["ENC1", "LPTIM1",  "id_encoder_1"],
        ["ENC2", "TIM1",    "id_encoder_2"],
        ["ENC3", "TIM5",    "id_encoder_3"],
        ["ENC4", "LPTIM2",  "id_encoder_4"],
        ["ENC5", "TIM4",    "id_encoder_5"],
        ["ENC6", "TIM8",    "id_encoder_6"],
        ["ENC7", "TIM3",    "id_encoder_7"]
    ];

    html.push("<div>");
    html.push("<h1>Encoders</h1>");
    html.push("<div>");
    html.push("<table>");

    for(let i=0; i<encoderAttributesList.length; i++)
    {
        attributes = encoderAttributesList[i];

        html.push(`<tr>`);
        html.push(`<td>${attributes[0]}</td>`);
        html.push(`<td>(${attributes[1]}):</td>`);
        html.push(`<td><label id="${attributes[2]}"></label></td>`);
        html.push(`</tr>`);
    }

    html.push("</table>");
    html.push("</div>");
    html.push("</div>");

    return html.join("");
}

function createTimerTable()
{
    let html = [];
    
    html.push("<div>");
    html.push("<h1>Timer</h1>");

    for(let i=0; i<6; i++)
    {
        html.push("<div>");
        html.push(`Flex Timer ${i}: prescaler:`);
        html.push(`<input type="number" id="id_flexTimer_prescaler_${i}" min="1" step="1" max="65536">`);
        html.push("</div>");
    }

    html.push("<div>");
    html.push(`Fixed Timer: prescaler:`);
    html.push(`<input type="number" id="id_fixTimer_prescaler" min="1" step="1" max="65536">`);
    html.push("</div>");
    
    html.push("</div>");

    return html.join("");   
}

function createStepperMode(stepperId)
{
    let html = [];

    html.push("<div><table>");

    // forced
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_mode_forced_${stepperId}" name="stepper_mode_${stepperId}">`);
        html.push(`<label for="id_stepper_mode_forced_${stepperId}">Forced</label>`)
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_mode_forced_${stepperId}'>`);
        html.push(`<label>Pulse period: </label><input type="number" id="id_stepper_period_force_${stepperId}" min="1" step="1" max="65536">`);
        html.push("</div>");
        html.push("</td>");
    }

    // active
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_active_forced_${stepperId}" name="stepper_active_${stepperId}">`);
        html.push(`<label for="id_stepper_mode_active_${stepperId}">Active</label>`)
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_mode_active_${stepperId}'>`);
        html.push(`<label>Starting pulse period: </label><input type="number" id="id_stepper_period_active_starting_${stepperId}" min="1" step="1" max="65536">`);
        html.push(`<label>Acceleration steps: </label><input type="numbe\" id="id_stepper_period_active_accelerationSteps_${stepperId}" min="1" step="1" max="1024">`);
        html.push("<br>");
        html.push(`<label>Cruising period: </label><input type="number" id="id_stepper_period_active_cruising_${stepperId}" min="1" step="1" max="65536">`);
        html.push("<br>");
        html.push(`<label>Ending pulse period: </label><input type="number" id="id_stepper_period_active_ending_${stepperId}" min="1" step="1" max="65536">`);
        html.push(`<label>Deacceleration steps: </label><input type="number" id="id_stepper_period_active_deaccelerationSteps_${stepperId}" min="1" step="1" max="1024">`);
        html.push("</div>")
        html.push("</td>");
    }

    // passive
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_passive_forced_${stepperId}" name="stepper_passive_${stepperId}">`);
        html.push(`<label for="id_stepper_mode_passive_${stepperId}">Passive</label>`)
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_mode_passive_${stepperId}'>`);
        html.push(`<label>ActiveStepper: </label><select id="id_stepper_period_passive_stepper_${stepperId}">`);
        for(let i=0; i<10; i++)
        {
            if(i == stepperId)
            {
                continue;
            }
            html.push(`<option value="${i}">Stepper ${i}</option>`);
        }
        html.push(`<option value="255" selected>Not selected</option>`);
        html.push("</div>")
        html.push("</td>");
    }

    html.push("</table></div>");

    return html.join("");   
}

function createStepperTable()
{
    let timerOptionList = [
        "Flex Timer 0",
        "Flex Timer 1",
        "Flex Timer 2",
        "Flex Timer 3",
        "Flex Timer 4",
        "Flex Timer 5",
        "Fixed Timer"
    ];

    let encoderOptionList = [
        "ENC0",
        "ENC1",
        "ENC2",
        "ENC3",
        "ENC4",
        "ENC5",
        "ENC6",
        "ENC7"
    ];

    let html = [];

    html.push("<div>");
    html.push("<h1>Stepper</h1>");
    html.push("<div>");
    
    for(let stepperIndex=0; stepperIndex<10; stepperIndex++)
    {
        html.push("<div>");
        html.push(`<h2>Stepper ${stepperIndex}</h2>`);
        html.push("<div>");
        {
            // alarm
            html.push("<div>");
            html.push("<label>Alarm:</label>");
            html.push(`<div class="unknown-dot" id="id_stepperAlarm_state_${stepperIndex}"></div>`);
            html.push("</div>");
            // disable, forward, clock
            html.push("<div><table><tr>");
            html.push(`<td><label>Disable<input type="checkbox" id="id_stepper_disable_${stepperIndex}"></label></td>`);
            html.push(`<td><label>Forward<input type="checkbox" id="id_stepper_forward_${stepperIndex}"></label></td>`);
            html.push(`<td><label>Clock<input type="checkbox" id="id_stepper_clock_${stepperIndex}"></label></td>`);
            html.push("</tr></table></div>");
            // timer
            html.push("<div>");
            html.push(`<label>Timer: </label><select id=\"id_stepper_timer_${stepperIndex}">`);
            for(let i=0; i<timerOptionList.length; i++)
            {
                html.push(`<option value="${timerOptionList[i]}">${timerOptionList[i]}</option>`);
            }
            html.push(`<option value="Not selected" selected>Not selected</option>`);
            html.push("</select>");
            html.push("</div>");
            // encoder
            html.push("<div>");
            html.push(`<label>Encoder: </label><select id=\"id_stepper_encoder_${stepperIndex}">`);
            for(let i=0; i<encoderOptionList.length; i++)
            {
                html.push(`<option value="${encoderOptionList[i]}">${encoderOptionList[i]}</option>`);
            }
            html.push(`<option value="Not selected" selected>Not selected</option>`);
            html.push("</select>");
            html.push("</div>");
            // mode
            html.push(createStepperMode(stepperIndex));
            // buttons
            html.push("<div>");
            html.push(`<button id="id_stepper_go_1_${stepperIndex}">1</button>`);
            html.push(`<button id="id_stepper_go_2_${stepperIndex}">2</button>`);
            html.push(`<button id="id_stepper_go_4_${stepperIndex}">4</button>`);
            html.push(`<button id="id_stepper_go_8_${stepperIndex}">8</button>`);
            html.push(`<button id="id_stepper_go_16_${stepperIndex}">16</button>`);
            html.push(`<button id="id_stepper_go_32_${stepperIndex}">32</button>`);
            html.push(`<button id="id_stepper_go_64_${stepperIndex}">64</button>`);
            html.push(`<input type="number" id="id_stepper_steps_${stepperIndex}" min="1" step="1" max="1024">`);
            html.push(`<button id="id_stepper_go_steps_${stepperIndex}">go</button>`);
            html.push("</div>");
        }
        html.push("</div>");
        html.push("</div>");
    }

    html.push("</div>");
    html.push("</div>");

    return html.join("");   
}

function createBody()
{
    let html = [];

    html.push(`<div>`);
    html.push("<h1>Version</h1>");
    html.push(`<label id="id_version"></label>`);
    html.push(`</div>`)

    html.push(createGpioTable());
    html.push(createPowerTable());
    html.push(createPositionDetectorTable());
    html.push(createBdcControlTable());
    html.push(createDynamicStatus());
    html.push(createEncoderTable());
    html.push(createTimerTable());
    html.push(createStepperTable());

    return html.join("");   
}

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

function getStepperMode(stepperId)
{
    if(stepperId < 0 || stepperId > 9)
    {
        return;
    }

    if(document.getElementById(`id_stepper_mode_forced_${stepperId}`).checked)
    {
        let e = document.getElementById(`id_stepper_period_force_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let period = Number(e.value);

        let mode = {
            mode: "forced",
            period: period
        }

        return mode;
    }
    else if (document.getElementById(`id_stepper_mode_active_${stepperId}`).checked)
    {
        let e = document.getElementById(`id_stepper_period_active_starting_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let startingPeriod = Number(e.value);

        e = document.getElementById(`id_stepper_period_active_accelerationSteps_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let accelerationSteps = Number(e.value);

        e = document.getElementById(`id_stepper_period_active_cruising_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let cruisingPeriod = Number(e.value);

        e = document.getElementById(`id_stepper_period_active_ending_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let endingPeriod = Number(e.value);

        e = document.getElementById(`id_stepper_period_active_deaccelerationSteps_${stepperId}`);
        if(!e.checkValidity())
        {
            return;
        }
        let deaccelerationSteps = Number(e.value);

        let mode = {
            mode: "active",
            startingPeriod: startingPeriod,
            accelerationSteps: accelerationSteps,
            cruisingPeriod: cruisingPeriod,
            deaccelerationSteps: deaccelerationSteps,
            endingPeriod: endingPeriod
        }

        return mode;
    }
    else if (document.getElementById(`id_stepper_mode_passive_${stepperId}`).checked)
    {
        let e = document.getElementById(`id_stepper_period_passive_stepper_${stepperId}`);
        let selection = e.value;
        let index = Number(selection);
        if(index == NaN)
        {
            return;
        }
        if(index < 0 || index > 9 || index == stepperId)
        {
            return;
        }

        let mode = {
            mode: "passive",
            activeStepperIndex: index
        }

        return mode;
    }
}

async function setStepper(id) 
{
    let segments = id.split("_");
    let action = segments[2];

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
        let stepperId = parseInt(segments[4], 10);
        let stepsNum = NaN;

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

        mode = getStepperMode(stepperId);
        if(!mode)
        {
            alert(`Error: invalid mode`);
            return;
        }

        payload = {
            index: stepperId,
            clocks: stepsNum,
            mode: mode
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
    else if(action == "mode")
    {
        let modeValue = segments[3];
        let stepperId = segments[4];

        let group = document.getElementById(`id_stepper_group_mode_forced_${stepperId}`);
        let elements = group.querySelectorAll("input, select, label");
        elements.forEach(el => el.disabled = true);

        group = document.getElementById(`id_stepper_group_mode_active_${stepperId}`);
        elements = group.querySelectorAll("input, select, label");
        elements.forEach(el => el.disabled = true);

        group = document.getElementById(`id_stepper_group_mode_passive_${stepperId}`);
        elements = group.querySelectorAll("input, select, label");
        elements.forEach(el => el.disabled = true);

        group = document.getElementById(`id_stepper_group_mode_${modeValue}_${stepperId}`);
        elements = group.querySelectorAll("input, select, label");
        elements.forEach(el => el.disabled = false);
    }
    else if(action == "period")
    {
        // do nothing
    }
    else if(action == "group")
    {
        // do nothing
    }
    else
    {
        alert(`Unsupported action '${action}' in '${id}'`);
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

    // refresh webpage
    await post('refreshStatus', {});
    await updateUI();
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

function updateGpio(status)
{
    let ports = status.gpioPorts;

    for(let portIndex = 0; portIndex < ports.length; portIndex++)
    {
        let port = ports[portIndex];
        let portName = "P" + String.fromCharCode('A'.charCodeAt(0) + portIndex);
        
        for(let bitIndex = 0; bitIndex < port.pins.length; bitIndex++)
        {
            let pin = port.pins[bitIndex];
            let labelId = `id_setGpio_${portName}_${bitIndex}_label`;
            let inputId = `id_setGpio_${portName}_${bitIndex}`;
            let label = document.getElementById(labelId);
            let input = document.getElementById(inputId);

            input.checked = pin.isHigh;

            label.className = pin.isWritable? "gpio-rw" : "gpio-ro";
            input.disabled = pin.isWritable? false : true;
        }
    }
}

function updatePowerOutput(status)
{
    let state;
    let checkbox;

    for(let i=0; i<status.powerOutputs.length; i++)
    {
        let stateId = `id_powerOutput_state_${i}`;
        let checkboxId = `id_powerOutput_set_${i}`;

        state = document.getElementById(stateId);
        checkbox = document.getElementById(checkboxId);

        state.className = status.powerOutputs[i].isOutputDetected ? "active-green-dot" : "inactive-green-dot";
        checkbox.checked = status.powerOutputs[i].isEnabled;
    }

    state = document.getElementById("id_bdcPowerMain_state");
    checkbox = document.getElementById("id_bdcPowerMain_set");

    state.className = status.bdcPowerOutput.isOutputDetected ? "active-green-dot" : "inactive-green-dot";
    checkbox.checked = status.bdcPowerOutput.isEnabled;
}

function updatePositionDetector(status)
{
    for(let i=0; i<status.positionDetectors.length; i++)
    {
        let state = document.getElementById(`id_positionDetector_${i}`);
        
        if(i < 20)
        {
            state.className = status.positionDetectors[i].isDetected ? "active-red-dot" : "inactive-red-dot";
        }
        else
        {
            state.className = status.positionDetectors[i].isDetected ? "active-green-dot" : "inactive-green-dot";
        }
    }
}

async function updateUI()
{
    let exception = false;
    let data;

    try {
        data = await get('Status');
    } catch (error) {
        exception = true;            
        console.error("Error:", error);
    }

    if(exception) {
        return;
    }

    let status;
    try {
        status = JSON.parse(data);
    } catch (e) {
        exception = true;
        console.error("Error: failed to parse status: ", e);
    }    
    if(exception) {
        return;
    }

    document.getElementById("id_version").textContent = status.virtualOperatorVersion;

    updateGpio(status);
    updatePowerOutput(status);
    updatePositionDetector(status);

}

// document.addEventListener('click', async function(event) { onDocumentClick(event); } );
let intervalId = setInterval(updateUI, 1000);

document.body.innerHTML = createBody();
