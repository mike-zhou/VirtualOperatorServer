
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
        html.push(`<label for="id_powerOutput_set_${i}"> Enable </label>`);
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
    html.push("<table>")

    html.push("<thead>");
    html.push("<tr>");
    html.push("<th>Name</th>");
    html.push("<th>State</th>");
    html.push("<th>Value</th>");
    html.push("<th>Config</th>");
    html.push("<th></th>");
    html.push("</tr>");
    html.push("</thead>");

    html.push("<tbody>");
    for(let i=0; i<6; i++)
    {
        html.push("<tr>");
        html.push(`<td>FLEX_TIMER_${i}:</td>`);
        html.push(`<td><label id='id_flexTimer_state_${i}'></label></td>`);
        html.push(`<td><label id="id_flexTimer_prescaler_value_${i}"></label></td>`);
        html.push(`<td><input type="number" id="id_flexTimer_prescaler_config_${i}" min="1" step="1" max="65536"></td>`);
        html.push(`<td><input type="button" id="id_flexTimer_prescaler_set_${i}" value="Set"></td>`);
        html.push(`<td><input type="button" id="id_flexTimer_prescaler_save_${i}" value="Save"></td>`);
        html.push("</tr>");
    }

    html.push("<tr>");
    html.push(`<td>FIX_TIMER:</td>`);
    html.push(`<td><label id='id_fixTimer_state'></label></td>`);
    html.push(`<td><label id="id_fixTimer_prescaler_value"></label></td>`);
    html.push(`<td><input type="number" id="id_fixTimer_prescaler_config" min="1" step="1" max="65536"></td>`);
    html.push(`<td><input type="button" id="id_fixTimer_prescaler_set" value="Set"></td>`);
    html.push(`<td><input type="button" id="id_fixTimer_prescaler_save" value="Save"></td>`);
    html.push("</tr>");
    
    html.push("</tbody>");
    html.push("</table>");
    html.push("</div>");

    return html.join("");   
}

function createStepperMode(stepperIndex)
{
    let html = [];

    html.push("<div><table>");

    // forced
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_mode_forced_${stepperIndex}" name="stepper_mode_${stepperIndex}">`);
        html.push(`<label for="id_stepper_mode_forced_${stepperIndex}">Forced</label>`);
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_forced_${stepperIndex}'>`);
        html.push(`<label>Pulse period: </label>`);
        html.push(`<label id="id_stepper_period_forced_value_${stepperIndex}" ></label>`);
        html.push(`<input type="number" id="id_stepper_period_forced_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_forced_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_forced_save_${stepperIndex}" value="Save">`);
        html.push("</div>");
        html.push("</td>");
    }

    // active
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_mode_active_${stepperIndex}" name="stepper_mode_${stepperIndex}">`);
        html.push(`<label for="id_stepper_mode_active_${stepperIndex}">Active</label>`);
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_active_${stepperIndex}'>`);

        html.push(`<label>Starting pulse period: </label>`);
        html.push(`<label id="id_stepper_period_active_starting_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_starting_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_starting_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_starting_save_${stepperIndex}" value="Save">`);
        html.push(`<label>Acceleration steps: </label>`);
        html.push(`<label id="id_stepper_period_active_accelerationSteps_value_${stepperIndex}"></label>`);
        html.push(`<input type="numbe\" id="id_stepper_period_active_accelerationSteps_config_${stepperIndex}" min="1" step="1" max="1024">`);
        html.push(`<input type="button" id="id_stepper_period_active_accelerationSteps_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_accelerationSteps_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label>Cruising period: </label>`);
        html.push(`<label id="id_stepper_period_active_cruising_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_cruising_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_cruising_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_cruising_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label>Ending pulse period: </label>`);
        html.push(`<label id="id_stepper_period_active_ending_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_ending_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_ending_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_ending_save_${stepperIndex}" value="Save">`);
        html.push(`<label>Deacceleration steps: </label>`);
        html.push(`<label id="id_stepper_period_active_deaccelerationSteps_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_deaccelerationSteps_config_${stepperIndex}" min="1" step="1" max="1024">`);
        html.push(`<input type="button" id="id_stepper_period_active_deaccelerationSteps_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_deaccelerationSteps_save_${stepperIndex}" value="Save">`);
        
        html.push("</div>")
        html.push("</td>");
    }

    // passive
    html.push("<tr>");
    {
        html.push("<td>");
        html.push(`<input type="radio" id="id_stepper_mode_passive_${stepperIndex}" name="stepper_mode_${stepperIndex}">`);
        html.push(`<label for="id_stepper_mode_passive_${stepperIndex}">Passive</label>`);
        html.push("</td>");
    }
    {
        html.push("<td>");
        html.push(`<div id='id_stepper_group_passive_${stepperIndex}'>`);
        html.push(`<label>ActiveStepper: </label>`);
        html.push(`<select id="id_stepper_passive_activeStepper_select_${stepperIndex}">`);
        for(let i=0; i<10; i++)
        {
            if(i == stepperIndex)
            {
                continue;
            }
            html.push(`<option value="STEPPER_${i}">STEPPER_${i}</option>`);
        }
        html.push(`<option value="NOT_SELECTED" selected>NOT_SELECTED</option>`);
        html.push("</div>")
        html.push("</td>");
    }

    html.push("</table></div>");

    return html.join("");   
}

function createStepperTable()
{
    let timerOptionList = [
        "FLEX_TIMER_0",
        "FLEX_TIMER_1",
        "FLEX_TIMER_2",
        "FLEX_TIMER_3",
        "FLEX_TIMER_4",
        "FLEX_TIMER_5",
        "FIX_TIMER"
    ];

    let encoderOptionList = [
        "ENC0",
        "ENC1",
        "ENC2",
        "ENC3",
        "ENC4",
        "ENC5",
        "ENC6",
        "ENC7",
        "NOT_SELECTED"
    ];

    let portList = [
        "PORT_A",
        "PORT_B",
        "PORT_C",
        "PORT_D",
        "PORT_E",
        "PORT_F",
        "PORT_G",
        "PORT_H",
        "PORT_I",
        "PORT_J",
        "PORT_K"
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
            // GPIO: disable, forward, clock
            html.push("<div><table><tr>");
            html.push(`<td><label>GpioDisable<input type="checkbox" id="id_stepper_gpio_disable_${stepperIndex}"></label></td>`);
            html.push(`<td><label>GpioForward<input type="checkbox" id="id_stepper_gpio_forward_${stepperIndex}"></label></td>`);
            html.push(`<td><label>GpioClock<input type="checkbox" id="id_stepper_gpio_clock_${stepperIndex}"></label></td>`);
            html.push("</tr></table></div>");
            // timer
            html.push("<div>");
            html.push(`<label>Timer: </label>`);
            html.push(`<label id='id_stepper_timer_${stepperIndex}'></label>`)
            html.push(`<select id="id_stepper_selectTimer_${stepperIndex}">`);
            for(let i=0; i<timerOptionList.length - 1; i++)
            {
                html.push(`<option value="${timerOptionList[i]}">${timerOptionList[i]}</option>`);
            }
            html.push(`<option value="${timerOptionList.at(-1)}" selected>${timerOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_setTimer_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_saveTimer_${stepperIndex}" value="Save"></td>`);
            html.push("</div>");
            // encoder
            html.push("<div>");
            html.push(`<label>Encoder: </label>`);
            html.push(`<label id='id_stepper_encoder_${stepperIndex}'></label>`);
            html.push(`<select id="id_stepper_selectEncoder_${stepperIndex}">`);
            for(let i=0; i<encoderOptionList.length - 1; i++)
            {
                html.push(`<option value="${encoderOptionList[i]}">${encoderOptionList[i]}</option>`);
            }
            html.push(`<option value="${encoderOptionList.at(-1)}" selected>${encoderOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_setEncoder_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_saveEncoder_${stepperIndex}" value="Save"></td>`);
            html.push("</div>");
            // mode
            html.push(createStepperMode(stepperIndex));
            // IsDisableHigh, IsForwardHigh, IsRisingEdgeDriven
            html.push("<div><table>");
            html.push(`<tr>`);
            html.push(`<td><label>IsDisableHigh<input type="checkbox" id="id_stepper_isDisableHigh_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isDisableHigh_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_isDisableHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsForwardHigh<input type="checkbox" id="id_stepper_isForwardHigh_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isForwardHigh_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_isForwardHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsRisingEdgeHigh<input type="checkbox" id="id_stepper_isRisingEdgeDriven_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isRisingEdgeDriven_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_isRisingEdgeDriven_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push("</table></div>");
            //
            html.push(`<div><table>`);
            html.push(`<tr>`);
            html.push(`<td><label>HomeBoundaryToReadySteps</label></td>`);
            html.push(`<td><label id='id_stepper_homeBoundaryToReadySteps_value_${stepperIndex}'></label></td>`);
            html.push(`<td><input type="number" id="id_stepper_homeBoundaryToReadySteps_config_${stepperIndex}" min="1" step="1" max="1024"></td>`);
            html.push(`<td><input type="button" id="id_stepper_homeBoundaryToReadySteps_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_homeBoundaryToReadySteps_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>Range</label></td>`);
            html.push(`<td><label id='id_stepper_range_value_${stepperIndex}'></label></td>`);
            html.push(`<td><input type="number" id="id_stepper_range_config_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_range_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_range_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>StepsPerRotation</label></td>`);
            html.push(`<td><label id='id_stepper_stepsPerRotation_value_${stepperIndex}'></label></td>`);
            html.push(`<td><input type="number" id="id_stepper_stepsPerRotation_config_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_stepsPerRotation_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_stepsPerRotation_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>EncoderCountsPerRotation</label></td>`);
            html.push(`<td><label id='id_stepper_encoderCountsPerRotation_value_${stepperIndex}'></label></td>`);
            html.push(`<td><input type="number" id="id_stepper_encoderCountsPerRotation_config_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderCountsPerRotation_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderCountsPerRotation_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>EncoderOffsetErrorThreshold</label></td>`);
            html.push(`<td><label id='id_stepper_encoderOffsetErrorThreshold_value_${stepperIndex}'></label></td>`);
            html.push(`<td><input type="number" id="id_stepper_encoderOffsetErrorThreshold_config_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderOffsetErrorThreshold_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderOffsetErrorThreshold_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push("</table></div>");

            {
                let nameList = [
                    "HomeBoundary",
                    "EndBoundary",
                    "Enable",
                    "Forward",
                    "Clock"
                ];

                html.push(`<div><table>`);

                for(let name of nameList)
                {
                    html.push(`<tr>`);

                    html.push(`<td><label>Port${name}</label></td>`);
                    html.push(`<td><label id='id_stepper_port${name}_value_${stepperIndex}'></label></td>`);
                    html.push(`<td>`);
                    html.push(`<select id="stepper_port${name}_select_${stepperIndex}">`);
                    for(let port of portList)
                    {
                        html.push(`<option value="${port}">${port}</option>`);
                    }
                    html.push(`</td>`);
                    html.push(`<td><input type="button" id="stepper_port${name}_set_${stepperIndex}" value="Set"></td>`);
                    html.push(`<td><input type="button" id="stepper_port${name}_save_${stepperIndex}" value="Save"></td>`);

                    html.push(`<td><label>Pin${name}</label></td>`);
                    html.push(`<td><label id='id_stepper_pin${name}_value_${stepperIndex}'></label></td>`);
                    html.push(`<td>`);
                    html.push(`<select id="stepper_pin${name}_select_${stepperIndex}">`);
                    for(let i=0; i<16; i++)
                    {
                        html.push(`<option value="${i}">${i}</option>`);
                    }
                    html.push(`</td>`);
                    html.push(`<td><input type="button" id="stepper_pin${name}_set_${stepperIndex}" value="Set"></td>`);
                    html.push(`<td><input type="button" id="stepper_pin${name}_save_${stepperIndex}" value="Save"></td>`);

                    html.push(`</tr>`);
                }

                html.push("</table></div>");
            }

            // control
            html.push("<div><table><tr>");
            html.push(`<td><label>Disable<input type="checkbox" id="id_stepper_control_disable_${stepperIndex}"></label></td>`);
            html.push(`<td><label>Forward<input type="checkbox" id="id_stepper_control_forward_${stepperIndex}"></label></td>`);
            html.push("</tr></table></div>");
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
    let segments = id.split("_");
    if( (segments.length < 4) ||
        (segments[0] != 'id') ||
        (segments[1] != 'setGpio'))
    
    {
        console.error(`Error: wrong element id '${id}' for setGpio`);
        return;
    }
    
    let newId = `${segments[0]}_${segments[1]}_${segments[2]}_${segments[3]}`;

    const checkbox = document.getElementById(newId);
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

async function setFlexTimerPrescaler(elementId)
{
    let segments = elementId.split("_");
    let action = segments[3];
    let timerIndex = Number(segments[4]);

    if(action == "set")
    {
        let configId = `${segments[0]}_${segments[1]}_${segments[2]}_config_${segments[4]}`;
        let value = Number(document.getElementById(configId).value);

        let payload = {
            timerId: timerIndex,
            prescaler: value
        };

        let data = await post('timerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of flex timer ${timerIndex}, info: ${data}`);
        }
    }
}

async function setFixTimerPrescaler(elementId)
{
    let segments = elementId.split("_");
    let action = segments[3];

    if(action == "set")
    {
        let configId = `${segments[0]}_${segments[1]}_${segments[2]}_config`;
        let value = Number(document.getElementById(configId).value);

        let payload = {
            timerId: 6,
            prescaler: value
        };

        let data = await post('timerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of fix timer, info: ${data}`);
        }
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

    if (elementId.startsWith("id_setGpio_"))
        await setGpio(elementId);
    else if (elementId.startsWith("id_powerOutput_set_"))
        await setPowerOutput(elementId);
    else if (elementId == "id_bdcPowerMain_set")
        await setBDCPowerOutput(elementId);
    else if (elementId.startsWith("id_bdcControl_"))
        await setBDCControl(elementId);
    else if (elementId.startsWith("id_flexTimer_prescaler_"))
        await setFlexTimerPrescaler(elementId);
    else if (elementId.startsWith("id_fixTimer_prescaler_"))
        await setFixTimerPrescaler(elementId);
    else if (elementId.startsWith("id_stepper_"))
        await setStepper(elementId);

    // refresh webpage
    await post('refreshStatus', {});
    await updateUI();
}

function onInputNumberFlexTimer(id)
{
    let segments = id.split("_");

    let valueId = `${segments[0]}_${segments[1]}_${segments[2]}_value_${segments[4]}`;
    let buttonId = `${segments[0]}_${segments[1]}_${segments[2]}_set_${segments[4]}`;
    let value = Number(document.getElementById(valueId).textContent);
    
    if(value == Number(document.getElementById(id).value))
    {
        document.getElementById(buttonId).disabled = true;
    }
    else
    {
        document.getElementById(buttonId).disabled = false;
    }
}

function onInputNumberFixTimer(id)
{
    let value = Number(document.getElementById("id_fixTimer_prescaler_value").textContent);
    
    if(value == Number(document.getElementById("id_fixTimer_prescaler_config").value))
    {
        document.getElementById("id_fixTimer_prescaler_set").disabled = true;
    }
    else
    {
        document.getElementById("id_fixTimer_prescaler_set").disabled = false;
    }
}

function onInputNumber(id)
{
    if(id.startsWith("id_flexTimer_prescaler_config_"))
    {
        onInputNumberFlexTimer(id);
    }
    else if(id.startsWith("id_fixTimer_prescaler_config"))
    {
        onInputNumberFixTimer(id);
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

function updateBdcControl(status)
{
    for(let i=0; i<status.bdcControls.length; i++)
    {
        let powerStateId = `id_bdcPowerOutput_state_${i}`;
        let coastId = `id_bdcControl_coast_${i}`;
        let reverseId = `id_bdcControl_reverse_${i}`;
        let forwardId = `id_bdcControl_forward_${i}`;
        let brakeId = `id_bdcControl_brake_${i}`;

        let powerState = document.getElementById(powerStateId);
        let coast = document.getElementById(coastId);
        let reverse = document.getElementById(reverseId);
        let forward = document.getElementById(forwardId);
        let brake = document.getElementById(brakeId);

        powerState.className = status.bdcControls[i].isPowerAvailable ? "active-green-dot" : "inactive-green-dot";

        switch(status.bdcControls[i].mode)
        {
            case 'COAST':
                coast.checked = true;
                break;
            case 'REVERSE':
                reverse.checked = true;
                break;
            case 'FORWARD':
                forward.checked = true;
                break;
            case 'BRAKE':
                brake.checked = true;
                break;
            default:
                console.error(`Error: unknown mode of ${i} BDC: `, status.bdcControls[i].mode);
                break;
        }
    }
}

function updateDynamicStatus(status)
{
    document.getElementById("id_dynamicStatus_mainLoop").textContent = 
        String(status.dynamicStatus.mainLoopPerSecond);
    
    document.getElementById("id_dynamicStatus_flexTimerIsrPeriod").textContent = 
        String(status.dynamicStatus.maxFlexTimerIsrPeriod);

    document.getElementById("id_dynamicStatus_fixTimerIsrPeriod").textContent = 
        String(status.dynamicStatus.maxFixTimerIsrPeriod);
}

function updateEncoder(status)
{
    for(let i=0; i<status.encoders.length; i++)
    {
        document.getElementById(`id_encoder_${i}`).textContent = 
            String(status.encoders[i].count);
    }
}

function updateTimer(status)
{
    for(let i=0; i<status.flexTimers.length; i++)
    {
        let stateId = `id_flexTimer_state_${i}`;
        let valueId = `id_flexTimer_prescaler_value_${i}`;
        let configId = `id_flexTimer_prescaler_config_${i}`;

        document.getElementById(stateId).textContent = status.flexTimers[i].state;
        document.getElementById(valueId).textContent = String(status.flexTimers[i].prescaler);
        let config = document.getElementById(configId);
        if(config.value === "")
        {
            config.value = status.flexTimers[i].prescalerConfig;
        }
    }

    document.getElementById("id_fixTimer_state").textContent = status.fixTimer.state;
    document.getElementById("id_fixTimer_prescaler_value").textContent = String(status.fixTimer.prescaler);
    if(document.getElementById("id_fixTimer_prescaler_config").value === "")
    {
        document.getElementById("id_fixTimer_prescaler_config").value = status.fixTimer.prescalerConfig;
    }
}

function updateStepper(status)
{
    for(let stepperIndex=0; stepperIndex<status.steppers.length; stepperIndex++)
    {
        let alarmId = `id_stepperAlarm_state_${stepperIndex}`;
        let gpioDisableId = `id_stepper_gpio_disable_${stepperIndex}`;
        let gpioForwardId = `id_stepper_gpio_forward_${stepperIndex}`;
        let gpioClockId = `id_stepper_gpio_clock_${stepperIndex}`;
        let timerId = `id_stepper_timer_${stepperIndex}`;
        let encoderId = `id_stepper_encoder_${stepperIndex}`;

        let forcedModePeriodValueId = `id_stepper_period_forced_value_${stepperIndex}`;
        
        let activeModeStartingPeriodValueId = `id_stepper_period_active_starting_value_${stepperIndex}`;
        let activeModeAccelerationStepsValueId = `id_stepper_period_active_accelerationSteps_value_${stepperIndex}`;
        let activeModeCruisePeriodValueId = `id_stepper_period_active_cruising_value_${stepperIndex}`;
        let activeModeEndingPeriodValueId = `id_stepper_period_active_ending_value_${stepperIndex}`;
        let activeModeDeaccelerationStepsValueId = `id_stepper_period_active_deaccelerationSteps_value_${stepperIndex}`;

        let passiveModeActiveStepperSelectId = `id_stepper_passive_activeStepper_select_${stepperIndex}`;

        let data = status.steppers[stepperIndex];

        // alarm
        document.getElementById(alarmId).className = data.isAlarmTriggered ? "active-red-dot" : "inactive-red-dot";
        // gpios
        document.getElementById(gpioDisableId).checked = data.gpios.isDisableHigh;
        document.getElementById(gpioForwardId).checked = data.gpios.isForwardHigh;
        document.getElementById(gpioClockId).checked = data.gpios.isClockHigh;
        // timer
        document.getElementById(timerId).textContent = data.config.timer;
        // encoder
        document.getElementById(encoderId).textContent = data.config.encoder;
        // forced mode
        document.getElementById(forcedModePeriodValueId).textContent = 
            String(status.steppers[stepperIndex].config.forcedModeConfig.pulseWidth);
        // active mode
        document.getElementById(activeModeStartingPeriodValueId).textContent =
            String(status.steppers[stepperIndex].config.activeModeConfig.startingPulseWidth);
        document.getElementById(activeModeAccelerationStepsValueId).textContent =
            String(status.steppers[stepperIndex].config.activeModeConfig.acceleratingSteps);
        document.getElementById(activeModeCruisePeriodValueId).textContent =
            String(status.steppers[stepperIndex].config.activeModeConfig.cruisingPulseWidth);
        document.getElementById(activeModeEndingPeriodValueId).textContent =
            String(status.steppers[stepperIndex].config.activeModeConfig.endingPulseWidth);
        document.getElementById(activeModeDeaccelerationStepsValueId).textContent =
            String(status.steppers[stepperIndex].config.activeModeConfig.deacceleratingSteps);
        
        // passive mode
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

    VO.status = status;
    document.getElementById("id_version").textContent = status.virtualOperatorVersion;

    updateGpio(status);
    updatePowerOutput(status);
    updatePositionDetector(status);
    updateBdcControl(status);
    updateDynamicStatus(status);
    updateEncoder(status);
    updateTimer(status);
    updateStepper(status);
}

const VO = {};

document.body.innerHTML = createBody();

document.addEventListener('click', async function(event) { onDocumentClick(event); } );
document.addEventListener('input', (e) => {
    const t = e.target;
    if (t instanceof HTMLInputElement && t.type === 'number') {
        const id = t.id;                  // ID of the input
        onInputNumber(id);
    }
});

let intervalId = setInterval(updateUI, 1000);

