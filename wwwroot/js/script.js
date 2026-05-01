
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

    html.push("<h2>Config</h2>");
    html.push("<table>")

    html.push("<thead>");
    html.push("<tr>");
    html.push("<th>Name</th>");
    html.push("<th>State</th>");
    html.push("<th>Value</th>");
    html.push("<th>Config</th>");
    html.push("<th></th>");
    html.push("<th></th>");
    html.push("<th>ClockWidth</th>");
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
        html.push(`<td><label id="id_flexTimer_prescaler_unit_${i}"></label></td>`);
        html.push(`<td> ns</td>`);
        html.push("</tr>");
    }

    html.push("<tr>");
    html.push(`<td>FIX_TIMER:</td>`);
    html.push(`<td><label id='id_fixTimer_state'></label></td>`);
    html.push(`<td><label id="id_fixTimer_prescaler_value"></label></td>`);
    html.push(`<td><input type="number" id="id_fixTimer_prescaler_config" min="1" step="1" max="65536"></td>`);
    html.push(`<td><input type="button" id="id_fixTimer_prescaler_set" value="Set"></td>`);
    html.push(`<td><input type="button" id="id_fixTimer_prescaler_save" value="Save"></td>`);
    html.push(`<td><label id="id_fixTimer_prescaler_unit"></label></td>`);
    html.push(`<td> ns</td>`);
    html.push("</tr>");
    
    html.push("</tbody>");
    html.push("</table>");

    html.push("<h2>Test</h2>");
    html.push("<table>");
    html.push("<thead>");
    html.push("<tr>");
    html.push("<th>Name</th>");
    html.push("<th>PulseWidth</th>");
    html.push("<th>TotalPulse</th>");
    html.push("<th>LogPeriod</th>");
    html.push("<th></th>");
    html.push("</tr>");
    html.push("</thead>");

    html.push("<tbody>");
    for(let i=0; i<6; i++)
    {
        html.push("<tr>");
        html.push(`<td>FLEX_TIMER_${i}:</td>`);
        html.push(`<td><input type="number" id="id_flexTimer_test_pulseWidth_${i}" min="1" step="1" max="65536"></td>`);
        html.push(`<td><input type="number" id="id_flexTimer_test_totalPulse_${i}" min="1" step="1" max="65536"></td>`);
        html.push(`<td><input type="number" id="id_flexTimer_test_logPeriod_${i}" min="1" step="1" max="65536"></td>`);
        html.push(`<td><input type="button" id="id_flexTimer_test_run_${i}" value="Run"></td>`);
        html.push("</tr>");
    }
    html.push("<tr>");
    html.push(`<td>FIX_TIMER:</td>`);
    html.push(`<td><input type="number" id="id_fixTimer_test_pulseWidth" min="1" step="1" max="65536"></td>`);
    html.push(`<td><input type="number" id="id_fixTimer_test_totalPulse" min="1" step="1" max="65536"></td>`);
    html.push(`<td><input type="number" id="id_fixTimer_test_logPeriod" min="1" step="1" max="65536"></td>`);
    html.push(`<td><input type="button" id="id_fixTimer_test_run" value="Run"></td>`);
    html.push("</tr>");
    html.push("</tbody>");
    html.push("</table>");

    html.push("</div>");

    return html.join("");   
}

function createStepperMode(stepperIndex)
{
    let html = [];

    html.push("<div><table style=\"box-shadow:2px 2px 4px rgba(0,0,0,0.25);\" onmouseenter=\"this.style.border='4px solid green';this.style.borderCollapse='collapse';for(const r of this.rows){for(const c of r.cells){c.style.border='1px solid green';}}\" onmouseleave=\"this.style.border='';this.style.borderCollapse='separate';for(const r of this.rows){for(const c of r.cells){c.style.border='';}}\">");

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
        html.push(`<label id="id_stepper_period_forced_name_${stepperIndex}">Pulse period: </label>`);
        html.push(`<input type="number" id="id_stepper_period_forced_config_${stepperIndex}" min="1" step="1" max="65536">`);
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

        html.push(`<label id="id_stepper_period_active_starting_name_${stepperIndex}">Starting pulse period: </label>`);
        html.push(`<input type="number" id="id_stepper_period_active_starting_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_starting_save_${stepperIndex}" value="Save">`);
        html.push(`<label id="id_stepper_period_active_accelerationSteps_name_${stepperIndex}">Acceleration steps: </label>`);
        html.push(`<input type="numbe\" id="id_stepper_period_active_accelerationSteps_config_${stepperIndex}" min="1" step="1" max="1024">`);
        html.push(`<input type="button" id="id_stepper_period_active_accelerationSteps_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label id="id_stepper_period_active_cruising_name_${stepperIndex}">Cruising period: </label>`);
        html.push(`<input type="number" id="id_stepper_period_active_cruising_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_cruising_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label id="id_stepper_period_active_ending_name_${stepperIndex}">Ending pulse period: </label>`);
        html.push(`<input type="number" id="id_stepper_period_active_ending_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_ending_save_${stepperIndex}" value="Save">`);
        html.push(`<label id="id_stepper_period_active_deaccelerationSteps_name_${stepperIndex}">Deacceleration steps: </label>`);
        html.push(`<input type="number" id="id_stepper_period_active_deaccelerationSteps_config_${stepperIndex}" min="1" step="1" max="1024">`);
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
        html.push("<div onmouseenter=\"this.style.backgroundColor='lightgreen'\" onmouseleave=\"this.style.backgroundColor=''\">");
        html.push(`<h2>Stepper ${stepperIndex}</h2>`);
        html.push("<div>");
        {
            // state
            html.push("<div>");
            html.push("<label>State: </label>");
            html.push(`<label id='id_stepper_state_${stepperIndex}'></label>`)
            html.push("</div>");
            html.push("<div>");
            html.push("<label> Offset: </label>");
            html.push(`<label id='id_stepper_offset_${stepperIndex}'></label>`)
            html.push("</div>");

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
            html.push(`<select id="id_stepper_timer_select_${stepperIndex}">`);
            for(let i=0; i<timerOptionList.length - 1; i++)
            {
                html.push(`<option value="${timerOptionList[i]}">${timerOptionList[i]}</option>`);
            }
            html.push(`<option value="${timerOptionList.at(-1)}" selected>${timerOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_timer_save_${stepperIndex}" value="Save"></td>`);
            html.push("</div>");

            // encoder
            html.push("<div>");
            html.push(`<label>Encoder: </label>`);
            html.push(`<select id="id_stepper_encoder_select_${stepperIndex}">`);
            for(let i=0; i<encoderOptionList.length - 1; i++)
            {
                html.push(`<option value="${encoderOptionList[i]}">${encoderOptionList[i]}</option>`);
            }
            html.push(`<option value="${encoderOptionList.at(-1)}" selected>${encoderOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_encoder_save_${stepperIndex}" value="Save"></td>`);
            html.push(`<label id="id_stepper_encoder_offset_${stepperIndex}"></label>`);
            html.push(`<label id="id_stepper_encoder_maxOffsetError_${stepperIndex}"></label>`);
            html.push("</div>");

            // mode
            html.push(createStepperMode(stepperIndex));

            // isEnableHigh, IsForwardHigh, IsRisingEdgeDriven
            html.push("<div><table>");
            html.push(`<tr>`);
            html.push(`<td><label>isEnableHigh<input type="checkbox" id="id_stepper_isEnableHigh_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isEnableHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsForwardHigh<input type="checkbox" id="id_stepper_isForwardHigh_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isForwardHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsRisingEdgeDriven<input type="checkbox" id="id_stepper_isRisingEdgeDriven_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isRisingEdgeDriven_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push("</table></div>");

            //
            html.push(`<div><table>`);
            html.push(`<tr>`);
            html.push(`<td><label>HomeBoundaryToReadySteps</label></td>`);
            html.push(`<td><input type="number" id="id_stepper_homeBoundaryToReadySteps_value_${stepperIndex}" min="1" step="1" max="1024"></td>`);
            html.push(`<td><input type="button" id="id_stepper_homeBoundaryToReadySteps_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>Range</label></td>`);
            html.push(`<td><input type="number" id="id_stepper_range_value_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_range_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>StepsPerRotation</label></td>`);
            html.push(`<td><input type="number" id="id_stepper_stepsPerRotation_value_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_stepsPerRotation_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>EncoderCountsPerRotation</label></td>`);
            html.push(`<td><input type="number" id="id_stepper_encoderCountsPerRotation_value_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderCountsPerRotation_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>EncoderOffsetErrorThreshold</label></td>`);
            html.push(`<td><input type="number" id="id_stepper_encoderOffsetErrorThreshold_value_${stepperIndex}" min="1" step="1" max="10240"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoderOffsetErrorThreshold_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push("</table></div>");

            // signal configurations
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
                    html.push(`<td>`);
                    {
                        html.push(`<select id="id_stepper_port_${name}_select_${stepperIndex}">`);
                        html.push(`<option value="" selected disabled>-- choose --</option>`);
                        for(let i=0; i<portList.length; i++)
                        {
                            html.push(`<option value="${portList[i]}">${portList[i]}</option>`);
                        }
                    }
                    html.push(`</td>`);
                    html.push(`<td><input type="button" id="id_stepper_port_${name}_save_${stepperIndex}" value="Save"></td>`);

                    html.push(`<td><label>Pin${name}</label></td>`);
                    html.push(`<td>`);
                    {
                        html.push(`<select id="id_stepper_pin_${name}_select_${stepperIndex}">`);
                        html.push(`<option value="" selected disabled>-- choose --</option>`);
                        for(let i=0; i<16; i++)
                        {
                            html.push(`<option value="${i}">${i}</option>`);
                        }
                    }
                    html.push(`</td>`);
                    html.push(`<td><input type="button" id="id_stepper_pin_${name}_save_${stepperIndex}" value="Save"></td>`);

                    html.push(`</tr>`);
                }

                html.push("</table></div>");
            }

            // cross boundary
            html.push('<div>');
            html.push(`<label>CrossBoundaryEnable<input type="checkbox" id="id_stepper_crossBoundary_enable_${stepperIndex}"></label>`);
            html.push(`<label id="id_stepper_crossBoundary_negativeRangeLabel_${stepperIndex}"> NegativeRange:</label>`);
            html.push(`<input type="number" id="id_stepper_crossBoundary_negativeRangeValue_${stepperIndex}" min="-32768" step="1" max="0">`);
            html.push(`<input type="button" id="id_stepper_crossBoundary_save_${stepperIndex}" value="Save">`);
            // boundary items
            html.push("<div><table>");
            for(let boundaryItemIndex = 0; boundaryItemIndex < 4; boundaryItemIndex++)
            {
                html.push("<tr>");
                html.push(`<td><label>BoundaryEnable_${boundaryItemIndex}<input type="checkbox" id="id_stepper_crossBoundaryItem_enable_${boundaryItemIndex}_${stepperIndex}"></label></td>`);
                html.push(`<td><input type="number" id="id_stepper_crossBoundaryItem_value_${boundaryItemIndex}_${stepperIndex}" step="1"></td>`);
                html.push(`<td><label id="id_stepper_crossBoundaryItem_errorLabel_${boundaryItemIndex}_${stepperIndex}"> error:</label></td>`);
                html.push(`<td><input type="number" id="id_stepper_crossBoundaryItem_error_${boundaryItemIndex}_${stepperIndex}"></td>`);
                html.push(`<td><input type="button" id="id_stepper_crossBoundaryItem_save_${boundaryItemIndex}_${stepperIndex}" value="Save">`);
                html.push("</tr>");
            }
            html.push("</table></div>");
            html.push('</div>');

            // set controls
            html.push('<div>');
            html.push(`<button id="id_stepper_setActivePeriods_${stepperIndex}">Set Active Periods</button>`);
            html.push(`<button id="id_stepper_setControls_${stepperIndex}">Set Controls</button>`);
            html.push('</div>');
            // control
            html.push("<div><table><tr>");
            html.push(`<td><label>Enable<input type="checkbox" id="id_stepper_control_enable_${stepperIndex}"></label></td>`);
            html.push(`<td><label>Forward<input type="checkbox" id="id_stepper_control_forward_${stepperIndex}"></label></td>`);
            html.push("</tr></table></div>");
            // home positioning
            html.push('<div>');
            html.push(`<button id="id_stepper_startHomePositioning_${stepperIndex}">Start Home Positioning</button>`);
            html.push('</div>');
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
            html.push(`<button id="id_stepper_setActiveSteps_${stepperIndex}">set active steps</button>`);
            html.push("</div>");
        }
        html.push("</div>");

        html.push(`<h3>Test</h3>`);
        html.push("<div>");
        {
            html.push(`<div><label>Test Stepper Enable<input type="checkbox" id="id_stepper_test_enable_${stepperIndex}"></label></div>`);
            html.push(`<div><label>Test Stepper Forward<input type="checkbox" id="id_stepper_test_forward_${stepperIndex}"></label></div>`);
            html.push(`<div><label>Test Stepper Clock<input type="checkbox" id="id_stepper_test_clock_${stepperIndex}"></label></div>`);
            // test force
            html.push('<div>');
            html.push("<label>Force: PulseWidth</label>");
            html.push(`<input type="number" id="id_stepper_test_force_pulseWidth_${stepperIndex}" min="1" step="1" max="10240">`);
            html.push("<label>Steps</label>");
            html.push(`<input type="number" id="id_stepper_test_force_steps_${stepperIndex}" min="1" step="1" max="1024">`);
            html.push(`<input type="button" id="id_stepper_test_force_set_${stepperIndex}" value="Set Force">`);
            html.push(`<input type="button" id="id_stepper_test_force_pulseEnd_${stepperIndex}" value="PulseEnd">`)
            html.push("</div>");
            // state ready
            html.push('<div>');
            html.push("<label>Active: </label>");
            html.push(`<input type="button" id="id_stepper_test_stateReady_${stepperIndex}" value="Set Ready">`)
            html.push(`<input type="number" id="id_stepper_test_active_steps_${stepperIndex}" min="1" step="1" max="10240">`);
            html.push(`<input type="button" id="id_stepper_test_active_set_${stepperIndex}" value="Set Active">`)
            html.push(`<input type="button" id="id_stepper_test_active_pulseEnd_${stepperIndex}" value="PulseEnd">`)
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

function getSync(endpoint)
{
    let request = new XMLHttpRequest();
    request.open('GET', `/get/${endpoint}`, false);
    request.send();

    if(request.status < 200 || request.status >= 300)
    {
        throw new Error(`Request failed with status ${request.status}`);
    }

    return request.responseText;
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

function initConfigWidgetsSteppers(status)
{
    for(let stepperIndex=0; stepperIndex<status.steppers.length; stepperIndex++)
    {
        let config = status.steppers[stepperIndex].config;

        document.getElementById(`id_stepper_timer_select_${stepperIndex}`).value = config.timer;
        document.getElementById(`id_stepper_encoder_select_${stepperIndex}`).value = config.encoder;
        
        document.getElementById(`id_stepper_period_forced_config_${stepperIndex}`).value = config.forcedModeConfig.pulseWidth;
        document.getElementById(`id_stepper_period_active_starting_config_${stepperIndex}`).value = config.activeModeConfig.startingPulseWidth;
        document.getElementById(`id_stepper_period_active_accelerationSteps_config_${stepperIndex}`).value = config.activeModeConfig.acceleratingSteps;
        document.getElementById(`id_stepper_period_active_cruising_config_${stepperIndex}`).value = config.activeModeConfig.cruisingPulseWidth;
        document.getElementById(`id_stepper_period_active_ending_config_${stepperIndex}`).value = config.activeModeConfig.endingPulseWidth;
        document.getElementById(`id_stepper_period_active_deaccelerationSteps_config_${stepperIndex}`).value = config.activeModeConfig.deacceleratingSteps;
        document.getElementById(`id_stepper_passive_activeStepper_select_${stepperIndex}`).value = config.passiveModeConfig.activeStepper;
        document.getElementById(`id_stepper_isEnableHigh_value_${stepperIndex}`).checked = config.isEnableHigh;
        document.getElementById(`id_stepper_isForwardHigh_value_${stepperIndex}`).checked = config.isForwardHigh;
        document.getElementById(`id_stepper_isRisingEdgeDriven_value_${stepperIndex}`).checked = config.isRisingEdgeDriven;
        document.getElementById(`id_stepper_homeBoundaryToReadySteps_value_${stepperIndex}`).valueAsNumber = config.homeBoundaryToReadySteps;
        document.getElementById(`id_stepper_range_value_${stepperIndex}`).valueAsNumber = config.range;
        document.getElementById(`id_stepper_stepsPerRotation_value_${stepperIndex}`).valueAsNumber = config.stepsPerRotation;
        document.getElementById(`id_stepper_encoderCountsPerRotation_value_${stepperIndex}`).valueAsNumber = config.encoderCountsPerRotation;
        document.getElementById(`id_stepper_encoderOffsetErrorThreshold_value_${stepperIndex}`).valueAsNumber = config.encoderOffsetErrorThreshold;
        document.getElementById(`id_stepper_port_HomeBoundary_select_${stepperIndex}`).value = config.portHomeBoundary;
        document.getElementById(`id_stepper_pin_HomeBoundary_select_${stepperIndex}`).value = config.pinHomeBoundary;
        document.getElementById(`id_stepper_port_EndBoundary_select_${stepperIndex}`).value = config.portEndBoundary;
        document.getElementById(`id_stepper_pin_EndBoundary_select_${stepperIndex}`).value = config.pinEndBoundary;
        document.getElementById(`id_stepper_port_Enable_select_${stepperIndex}`).value = config.portEnable;
        document.getElementById(`id_stepper_pin_Enable_select_${stepperIndex}`).value = config.pinEnable;
        document.getElementById(`id_stepper_port_Forward_select_${stepperIndex}`).value =  config.portForward;
        document.getElementById(`id_stepper_pin_Forward_select_${stepperIndex}`).value = config.pinForward;
        document.getElementById(`id_stepper_port_Clock_select_${stepperIndex}`).value = config.portClock;
        document.getElementById(`id_stepper_pin_Clock_select_${stepperIndex}`).value = config.pinClock;

        document.getElementById(`id_stepper_crossBoundary_enable_${stepperIndex}`).checked = config.crossBoundary.enabled;
        document.getElementById(`id_stepper_crossBoundary_negativeRangeValue_${stepperIndex}`).value = config.crossBoundary.negativeRange;
        for(let boundaryIndex = 0; boundaryIndex < config.crossBoundary.boundaries.length; boundaryIndex++)
        {
            let boundaryConfig = config.crossBoundary.boundaries[boundaryIndex];
            let boundaryEnable = document.getElementById(`id_stepper_crossBoundaryItem_enable_${boundaryIndex}_${stepperIndex}`);
            let boundaryValue = document.getElementById(`id_stepper_crossBoundaryItem_value_${boundaryIndex}_${stepperIndex}`);
            let boundaryError = document.getElementById(`id_stepper_crossBoundaryItem_error_${boundaryIndex}_${stepperIndex}`);

            boundaryEnable.checked = boundaryConfig.enabled;
            boundaryValue.valueAsNumber = boundaryConfig.value;
            boundaryError.valueAsNumber = boundaryConfig.error;
        }
    }
}

function initConfigWidgetsTimers(status)
{

}

function initConfigWidgets()
{
    let status;

    try {
        let data = getSync('Status');
        status = JSON.parse(data);
        VO.status = status;
    } catch (error) {
        console.error("Error: failure in getting configurations: ", error);
        return;
    }

    initConfigWidgetsSteppers(status);
    initConfigWidgetsTimers(status);
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

async function onClick_Gpio(id)
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

    let payload = [
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

async function onClick_PowerOutput(id) 
{
    segments = id.split('_');

    powerNumber = segments[segments.length - 1];
    enablePower = document.getElementById(id).checked;

    let payload = 
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

async function onClick_BDCPowerOutput(id) 
{
    enablePower = document.getElementById(id).checked;

    let payload = 
        { 
            enable: enablePower
        };
        
    data = await post('setBDCPowerOutput', payload);
    if(data != "success")
    {
        console.error(`Error: failed to set PO ${powerNumber}, info: ${data}`);
    }
}

async function onClick_BDCControl(id) 
{
    segments = id.split("_");
    actionStr = segments[2];
    indexStr = segments[3];

    let payload = 
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

async function onClick_FlexTimerPrescaler(elementId)
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

        let data = await post('setTimerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of flex timer ${timerIndex}, info: ${data}`);
        }
    }
    else if(action == 'save')
    {
        let configId = `${segments[0]}_${segments[1]}_${segments[2]}_config_${segments[4]}`;
        let value = Number(document.getElementById(configId).value);

        let payload = {
            timerId: timerIndex,
            prescaler: value
        };

        let data = await post('saveTimerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of flex timer ${timerIndex}, info: ${data}`);
        }
    }
}

async function onClick_FlexTimerTest(elementId)
{
    let segments = elementId.split("_");
    let action = segments[3];
    let timerIndex = Number(segments[4]);

    if(action == "run")
    {
        let pulseWidthId = `${segments[0]}_${segments[1]}_${segments[2]}_pulseWidth_${segments[4]}`;
        let totalPulseId = `${segments[0]}_${segments[1]}_${segments[2]}_totalPulse_${segments[4]}`;
        let logPeriodId = `${segments[0]}_${segments[1]}_${segments[2]}_logPeriod_${segments[4]}`;

        let pulseWidth = document.getElementById(pulseWidthId).value;
        let totalPulse = document.getElementById(totalPulseId).value;
        let logPeriod = document.getElementById(logPeriodId).value;

        let payload = {
            timerId: timerIndex,
            pulseWidth: Number(pulseWidth),
            totalPulse: Number(totalPulse),
            logPeriod: Number(logPeriod)
        };

        let data = await post('testTimer', payload);
        if(data != "success")
        {
            alert(`Error: failed to test flex timer ${timerIndex}, info: ${data}`);
        }
    }
}

async function onClick_FlexTimer(elementId)
{
    let segments = elementId.split("_");
    let classification = segments[2];

    if(classification == "prescaler")
    {
        await onClick_FlexTimerPrescaler(elementId);
    }
    else if(classification == "test")
    {
        await onClick_FlexTimerTest(elementId);
    }
}

async function onClick_FixTimerPrescaler(elementId)
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

        let data = await post('setTimerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of fix timer, info: ${data}`);
        }
    }
    else if(action == "save")
    {
        let configId = `${segments[0]}_${segments[1]}_${segments[2]}_config`;
        let value = Number(document.getElementById(configId).value);

        let payload = {
            timerId: 6,
            prescaler: value
        };

        let data = await post('saveTimerPrescaler', payload);
        if(data != "success")
        {
            alert(`Error: failed to prescaler of fix timer, info: ${data}`);
        }
    }
}

async function onClick_FixTimerTest(elementId)
{
    let segments = elementId.split("_");
    let action = segments[3];

    if(action == "run")
    {
        let pulseWidthId = `${segments[0]}_${segments[1]}_${segments[2]}_pulseWidth`;
        let totalPulseId = `${segments[0]}_${segments[1]}_${segments[2]}_totalPulse`;
        let logPeriodId = `${segments[0]}_${segments[1]}_${segments[2]}_logPeriod`;

        let pulseWidth = document.getElementById(pulseWidthId).value;
        let totalPulse = document.getElementById(totalPulseId).value;
        let logPeriod = document.getElementById(logPeriodId).value;

        let payload = {
            timerId: 6,
            pulseWidth: Number(pulseWidth),
            totalPulse: Number(totalPulse),
            logPeriod: Number(logPeriod)
        };

        let data = await post('testTimer', payload);
        if(data != "success")
        {
            alert(`Error: failed to test flex timer ${timerIndex}, info: ${data}`);
        }
    }
}

async function onClick_FixTimer(elementId)
{
    let segments = elementId.split("_");
    let classification = segments[2];

    if(classification == "prescaler")
    {
        await onClick_FixTimerPrescaler(elementId);
    }
    else if(classification == "test")
    {
        await onClick_FixTimerTest(elementId);
    }
}

async function onClick_Stepper(id) 
{
    let segments = id.split("_");
    let classification = segments[2];

    if(classification == "gpio")
    {
        let action = segments[3];
        let stepperId = segments[4];

        if(action == "disable")
        {
            let payload = {
                index: parseInt(stepperId, 10),
                disableStepper: document.getElementById(id).checked
            }

            let data = await post('disableStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to enabl stepper ${stepperId}, info: ${data}`);
            }
        }
        else if(action == "forward")
        {
            let payload = {
                index: parseInt(stepperId, 10),
                forwardStepper: document.getElementById(id).checked
            }

            let data = await post('forwardStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to forward stepper ${stepperId}, info: ${data}`);
            }
        }
        else if(action == "clock")
        {
            let payload = {
                index: parseInt(stepperId, 10),
                highLevel: document.getElementById(id).checked
            }

            let data = await post('clockStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to clock stepper ${stepperId}, info: ${data}`);
            }
        }
    }
    else if(classification == "timer")
    {
        let action = segments[3];

        if(action == "save")
        {
            let stepperIndex = Number(segments[4]);
            let selectId = `id_stepper_timer_select_${stepperIndex}`;
            let timer = document.getElementById(selectId).value;

            let payload = {
                stepperId: stepperIndex,
                timer: timer
            }

            let data = await post('saveStepperConfigTimer', payload);
            if(data != "success")
            {
                alert(`Error: failed save timer, info: ${data}`);
            }
        }
    }
    else if(classification == "encoder")
    {
        let action = segments[3];

        if(action == "save")
        {
            let stepperIndex = Number(segments[4]);
            let selectId = `id_stepper_encoder_select_${stepperIndex}`;
            let encoder = document.getElementById(selectId).value;

            let payload = {
                stepperId: stepperIndex,
                encoder: encoder
            }

            let data = await post('saveStepperConfigEncoder', payload);
            if(data != "success")
            {
                alert(`Error: failed save encoder, info: ${data}`);
            }
        }
    }
    else if(classification == "period")
    {
        let mode = segments[3];

        let payload;
        let errString = "";

        if(mode == "forced")
        {
            let action = segments[4];
            let stepperIndex = segments[5];
            if(action == "save")
            {
                let periodId = `id_stepper_period_forced_config_${stepperIndex}`;
                let period = document.getElementById(periodId).value;

                payload = {
                    stepperId: Number(stepperIndex),
                    mode: mode,
                    value: Number(period)
                }
                errString = `Error: failed save forced period`;
            }
        }
        else if(mode == "active")
        {
            let type = segments[4];
            let action = segments[5];
            let stepperIndex = segments[6];

            if(type == "starting")
            {
                if(action == "save")
                {
                    let periodId = `id_stepper_period_active_starting_config_${stepperIndex}`;
                    let period = document.getElementById(periodId).value;

                    payload = {
                        stepperId: Number(stepperIndex),
                        mode: mode,
                        type: type,
                        value: Number(period)
                    }
                    errString = `Error: failed save active starting period`;
                }
            }
            else if(type == "accelerationSteps")
            {
                if(action == "save")
                {
                    let stepsId = `id_stepper_period_active_accelerationSteps_config_${stepperIndex}`;
                    let steps = document.getElementById(stepsId).value;

                    payload = {
                        stepperId: Number(stepperIndex),
                        mode: mode,
                        type: type,
                        value: Number(steps)
                    }
                    errString = `Error: failed to save active acceleration steps`;
                }
            }
            else if(type == "cruising")
            {
                if(action == "save")
                {
                    let periodId = `id_stepper_period_active_cruising_config_${stepperIndex}`;
                    let period = document.getElementById(periodId).value;

                    payload = {
                        stepperId: Number(stepperIndex),
                        mode: mode,
                        type: type,
                        value: Number(period)
                    }
                    errString = `Error: failed save active cruising period`;
                }
            }
            else if(type == "ending")
            {
                if(action == "save")
                {
                    let periodId = `id_stepper_period_active_ending_config_${stepperIndex}`;
                    let period = document.getElementById(periodId).value;

                    payload = {
                        stepperId: Number(stepperIndex),
                        mode: mode,
                        type: type,
                        value: Number(period)
                    }
                    errString = `Error: failed save active ending period`;
                }
            }
            else if(type == "deaccelerationSteps")
            {
                if(action == "save")
                {
                    let stepsId = `id_stepper_period_active_deaccelerationSteps_config_${stepperIndex}`;
                    let steps = document.getElementById(stepsId).value;

                    payload = {
                        stepperId: Number(stepperIndex),
                        mode: mode,
                        type: type,
                        value: Number(steps)
                    }
                    errString = `Error: failed to save active deacceleration steps`;
                }
            }
        }

        if(payload != undefined)
        {
            let data = await post('saveStepperConfigMode', payload);
            if(data != "success")
            {
                alert(`${errString}, info: ${data}`);
            }
        }
    }
    else if(classification == "passive")
    {
        // ignore passive settings
    }
    else if(classification == "mode")
    {
        let modeValue = segments[3];
        let stepperId = segments[4];

        let group;
        let elements;

        // disable all groups
        group = document.getElementById(`id_stepper_group_forced_${stepperId}`);
        elements = group.querySelectorAll("input, select");
        elements.forEach(el => el.disabled = true);
        elements = group.querySelectorAll("label");
        elements.forEach(el => el.className = "disabled-label");


        group = document.getElementById(`id_stepper_group_active_${stepperId}`);
        elements = group.querySelectorAll("input, select");
        elements.forEach(el => el.disabled = true);
        elements = group.querySelectorAll("label");
        elements.forEach(el => el.className = "disabled-label");

        group = document.getElementById(`id_stepper_group_passive_${stepperId}`);
        elements = group.querySelectorAll("input, select");
        elements.forEach(el => el.disabled = true);
        elements = group.querySelectorAll("label");
        elements.forEach(el => el.className = "disabled-label");

        // enable the chosen group
        group = document.getElementById(`id_stepper_group_${modeValue}_${stepperId}`);
        elements = group.querySelectorAll("input, select");
        elements.forEach(el => el.disabled = false);
        elements = group.querySelectorAll("label");
        elements.forEach(el => el.className = "");
    }
    else if(classification == "group")
    {
        // do nothing
    }
    else if((classification == "isEnableHigh") ||
            (classification == "isForwardHigh") ||
            (classification == "isRisingEdgeDriven"))
    {
        let action = segments[3];

        if(action == 'save')
        {
            let stepperIndex = segments[4];
            let checkId = `id_stepper_${classification}_value_${stepperIndex}`;
            let checked = document.getElementById(checkId).checked;

            let payload = {
                stepperId: Number(stepperIndex),
                classification: classification,
                isChecked: checked
            }

            let data = await post('saveStepperConfig', payload);
            if(data != "success")
            {
                alert(`Error: failed to save ${classification}, info: ${data}`);
            }
        }
    }
    else if((classification == "homeBoundaryToReadySteps") ||
            (classification == "range") ||
            (classification == "stepsPerRotation") ||
            (classification == "encoderCountsPerRotation") ||
            (classification == "encoderOffsetErrorThreshold"))
    {
        let action = segments[3];

        if(action == 'save')
        {
            let stepperIndex = segments[4];
            let valueId = `id_stepper_${classification}_value_${stepperIndex}`;
            let value = document.getElementById(valueId).valueAsNumber;

            let payload = {
                stepperId: Number(stepperIndex),
                classification: classification,
                value: value
            }

            let data = await post('saveStepperConfig', payload);
            if(data != "success")
            {
                alert(`Error: failed to save ${classification}, info: ${data}`);
            }
        }
    }
    else if((classification == "port") ||
            (classification == "pin"))
    {
        let action = segments[4];

        if(action == 'save')
        {
            let name = segments[3];
            let stepperIndex = segments[5];

            let selectionId = `id_stepper_${classification}_${name}_select_${stepperIndex}`;
            let value = document.getElementById(selectionId).value;

            let payload = {
                stepperId: Number(stepperIndex),
                classification: classification,
                name: name,
                value: value
            }
                
            let data = await post('saveStepperConfig', payload);
            if(data != "success")
            {
                alert(`Error: failed to save ${classification}, info: ${data}`);
            }
        }
    }
    else if(classification == "crossBoundary")
    {
        let action = segments[3];

        if(action == "save")
        {
            let stepperIndex = segments[4];
            let isEnabled = document.getElementById(`id_stepper_crossBoundary_enable_${stepperIndex}`).checked;
            let negativeRange = document.getElementById(`id_stepper_crossBoundary_negativeRangeValue_${stepperIndex}`).value;
            let payload = {
                stepperId: Number(stepperIndex),
                classification: classification,
                enabled: isEnabled,
                negativeRange: Number(negativeRange)
            }

            let data = await post('saveStepperConfig', payload);
            if(data != "success")
            {
                alert(`Error: failed to save ${classification}, info: ${data}`);
            }
        }
    }
    else if(classification == "crossBoundaryItem")
    {
        let action = segments[3];

        if(action == "save")
        {
            let itemIndex = segments[4];
            let stepperIndex = segments[5];

            let isEnabled = document.getElementById(`id_stepper_crossBoundaryItem_enable_${itemIndex}_${stepperIndex}`).checked;
            let value = document.getElementById(`id_stepper_crossBoundaryItem_value_${itemIndex}_${stepperIndex}`).value;
            let error = document.getElementById(`id_stepper_crossBoundaryItem_error_${itemIndex}_${stepperIndex}`).value;

            let payload = {
                stepperId: Number(stepperIndex),
                classification: classification,
                itemIndex: Number(itemIndex),
                isEnabled: isEnabled,
                boundaryValue: Number(value),
                boundaryError: Number(error)
            }

            let data = await post('saveStepperConfig', payload);
            if(data != "success")
            {
                alert(`Error: failed to save ${classification}, info: ${data}`);
            }
        }
    }
    else if(classification == "setActivePeriods")
    {
        let stepperIndex = parseInt(segments[3], 10);
        let payload = {
            stepperId: stepperIndex
        }

        let data = await post('setActivePeriods', payload);
        if(data != "success")
        {
            alert(`Error: failed to set active periods, info: ${data}`);
        }
    }
    else if(classification == "setControls")
    {
        let stepperIndex = parseInt(segments[3], 10);
        let payload = {
            stepperId: stepperIndex
        }

        let data = await post('setStepperControls', payload);
        if(data != "success")
        {
            alert(`Error: failed to set controls, info: ${data}`);
        }
    }
    else if(classification == "control")
    {
        let action = segments[3];

        if(action == 'enable')
        {
            let stepperIndex = parseInt(segments[4], 10);
            let checked = document.getElementById(id).checked;
            let payload = {
                stepperId: stepperIndex,
                enable: checked
            }

            let data = await post('setStepperEnable', payload);
            if(data != "success")
            {
                alert(`Error: failed to set stepper disable, info: ${data}`);
            }
        }
        else if(action == 'forward')
        {
            let stepperIndex = parseInt(segments[4], 10);
            let checked = document.getElementById(id).checked;
            let payload = {
                stepperId: stepperIndex,
                forward: checked
            }

            let data = await post('setStepperForward', payload);
            if(data != "success")
            {
                alert(`Error: failed to set stepper forward, info: ${data}`);
            }
        }
        else
        {
            alert(`Unsupported action '${action}' in '${id}'`);
        }
    }
    else if(classification == "startHomePositioning")
    {
        let stepperIndex = parseInt(segments[3], 10);
        let payload = {
            stepperId: stepperIndex
        }

        let data = await post('startStepperHomePositioning', payload);
        if(data != "success")
        {
            alert(`Error: failed to start stepper home positioning, info: ${data}`);
        }
    }
    else if(classification == "steps")
    {
        // do nothing
    }
    else if(classification == "go")
    {
        let stepperIndex = parseInt(segments[4], 10);
        let stepsNum = NaN;

        let steps = segments[3];
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

        if((stepsNum < 0) || (stepsNum > 2048))
        {
            alert(`Error: Out of range of steps: ${stepsNum}`);
            return;
        }

        let mode = "";

        if(document.getElementById(`id_stepper_mode_forced_${stepperIndex}`).checked)
        {
            mode = "forced";
        }
        else if(document.getElementById(`id_stepper_mode_active_${stepperIndex}`).checked)
        {
            mode = "active";
        }
        else if(document.getElementById(`id_stepper_mode_passive_${stepperIndex}`).checked)
        {
            mode = "passive";
        }

        if(mode == "")
        {
            alert(`Error: invalid mode`);
            return;
        }

        let forward = document.getElementById(`id_stepper_control_forward_${stepperIndex}`).checked;

        let payload = {
            stepperId: Number(stepperIndex),
            mode: mode,
            forward: forward,
            steps: stepsNum
        }

        if(mode == "passive")
        {
            let activeStepper = document.getElementById(`id_stepper_passive_activeStepper_select_${stepperIndex}`).value;
            
            if(activeStepper == "NOT_SELECTED")
            {
                alert(`Error: active stepper is not selected in passive mode`);
                return;
            }
            payload.activeStepperId = Number(activeStepper.split('_')[1]);

            let activeStepperStepsId = `id_stepper_steps_${payload.activeStepperId}`;
            let activeSteps = document.getElementById(activeStepperStepsId).value;
            if(Number.isNaN(activeSteps))
            {
                alert(`Error: No valid number in id_stepper_steps_${payload.activeStepperId}`);
                return;
            }
            activeSteps = parseInt(activeSteps, 10);
            payload.activeSteps = activeSteps;
        }

        let data = await post('runStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to run stepper ${segments[4]}, info: ${data}`);
        }
    }
    else if(classification == "setActiveSteps")
    {
        let stepperIndex = segments[3];
        let inputId = `id_stepper_steps_${stepperIndex}`;
        let steps = document.getElementById(inputId).value;

        if(Number.isNaN(steps))
        {
            alert(`Error: No valid number in id_stepper_steps_${stepperIndex}`);
            return;
        }
        steps = parseInt(steps, 10);

        if(!document.getElementById(`id_stepper_mode_active_${stepperIndex}`).checked)
        {
            alert(`Error: mode of stepper ${stepperIndex} is not active`);
            return;
        }

        let payload = {
            stepperId: Number(stepperIndex),
            steps: steps
        }

        let data = await post('setActiveSteps', payload);
        if(data != "success")
        {
            alert(`Error: failed to set active steps on stepper ${stepperIndex}, info: ${data}`);
        }
    }
    else if(classification == "test")
    {
        let action = segments[3];

        if(action == "enable")
        {
            let stepperIndex = Number(segments[4]);
            let checked = document.getElementById(id).checked;

            let payload = {
                stepperId: stepperIndex,
                isEnable: checked
            }

            let data = await post('testStepperEnable', payload);
            if(data != "success")
            {
                alert(`Failed in testStepperEnable, info: ${data}`);
            }
        }
        else if(action == "forward")
        {
            let stepperIndex = Number(segments[4]);
            let checked = document.getElementById(id).checked;

            let payload = {
                stepperId: stepperIndex,
                isForward: checked
            }

            let data = await post('testStepperForward', payload);
            if(data != "success")
            {
                alert(`Failed in testStepperForward, info: ${data}`);
            }
        }
        else if(action == "clock")
        {
            let stepperIndex = Number(segments[4]);
            let checked = document.getElementById(id).checked;

            let payload = {
                stepperId: stepperIndex,
                isFirstHalf: checked
            }

            let data = await post('testStepperClock', payload);
            if(data != "success")
            {
                alert(`Failed in testStepperClock, info: ${data}`);
            }
        }
        else if(action == "force")
        {
            let button = segments[4];
            let stepperIndex = Number(segments[5]);

            if(button == "set")
            {
                let pulseWidthId = `${segments[0]}_${segments[1]}_${segments[2]}_${segments[3]}_pulseWidth_${segments[5]}`;
                let stepsId = `${segments[0]}_${segments[1]}_${segments[2]}_${segments[3]}_steps_${segments[5]}`;

                let pulseWidth = document.getElementById(pulseWidthId).value;
                let steps = document.getElementById(stepsId).value;

                let payload = {
                    stepperId: stepperIndex,
                    pulseWidth: Number(pulseWidth),
                    steps: Number(steps)
                }

                let data = await post('testStepperForce', payload);
                if(data != "success")
                {
                    alert(`Failed in testStepperForce, info: ${data}`);
                }
            }
            else if(button == "pulseEnd")
            {
                let payload = {
                    stepperId: stepperIndex
                }

                let data = await post('testStepperPulseEnd', payload);
                if(data != "success")
                {
                    alert(`Failed in testStepperPulseEnd, info: ${data}`);
                }
            }
        }
        else if(action == "stateReady")
        {
            let stepperIndex = Number(segments[4]);
            let payload = {
                stepperId: stepperIndex
            }

            let data = await post('testStepperStateReady', payload);
            if(data != "success")
            {
                alert(`Failed in testStepperForce, info: ${data}`);
            }
        }
        else if(action == "active")
        {
            let button = segments[4];
            let stepperIndex = Number(segments[5]);

            if(button == "set")
            {
                let stepsId = `${segments[0]}_${segments[1]}_${segments[2]}_${segments[3]}_steps_${segments[5]}`;

                let steps = document.getElementById(stepsId).value;

                let payload = {
                    stepperId: stepperIndex,
                    steps: Number(steps)
                }

                let data = await post('testStepperActive', payload);
                if(data != "success")
                {
                    alert(`Failed in testStepperActive, info: ${data}`);
                }
            }
            else if(button == "pulseEnd")
            {
                let payload = {
                    stepperId: stepperIndex
                }

                let data = await post('testStepperPulseEnd', payload);
                if(data != "success")
                {
                    alert(`Failed in testStepperPulseEnd, info: ${data}`);
                }
            }
        }
    }
    else
    {
        alert(`Unsupported classification '${classification}' in '${id}'`);
    }
}

async function onClick(event)
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
        await onClick_Gpio(elementId);
    else if (elementId.startsWith("id_powerOutput_set_"))
        await onClick_PowerOutput(elementId);
    else if (elementId == "id_bdcPowerMain_set")
        await onClick_BDCPowerOutput(elementId);
    else if (elementId.startsWith("id_bdcControl_"))
        await onClick_BDCControl(elementId);
    else if (elementId.startsWith("id_flexTimer_"))
        await onClick_FlexTimer(elementId);
    else if (elementId.startsWith("id_fixTimer_"))
        await onClick_FixTimer(elementId);
    else if (elementId.startsWith("id_stepper_"))
        await onClick_Stepper(elementId);

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
        let unitId = `id_flexTimer_prescaler_unit_${i}`;

        document.getElementById(stateId).textContent = status.flexTimers[i].state;
        document.getElementById(valueId).textContent = String(status.flexTimers[i].prescaler);
        let config = document.getElementById(configId);
        if(config.value === "")
        {
            config.value = status.flexTimers[i].prescalerConfig;
        }

        document.getElementById(unitId).textContent = Math.round(status.flexTimers[i].prescaler * 4.16).toLocaleString('en-US'); 
    }

    document.getElementById("id_fixTimer_state").textContent = status.fixTimer.state;
    document.getElementById("id_fixTimer_prescaler_value").textContent = String(status.fixTimer.prescaler);
    if(document.getElementById("id_fixTimer_prescaler_config").value === "")
    {
        document.getElementById("id_fixTimer_prescaler_config").value = status.fixTimer.prescalerConfig;
    }

    document.getElementById("id_fixTimer_prescaler_unit").textContent = Math.round(status.fixTimer.prescaler * 17).toLocaleString('en-US'); 
}

function updateStepper(status)
{
    for(let stepperIndex=0; stepperIndex<status.steppers.length; stepperIndex++)
    {
        let stepper = status.steppers[stepperIndex];

        // state
        document.getElementById(`id_stepper_state_${stepperIndex}`).textContent = stepper.status.state;

        //offset
        document.getElementById(`id_stepper_offset_${stepperIndex}`).textContent = stepper.status.offset;

        // alarm
        let alarmId = `id_stepperAlarm_state_${stepperIndex}`;
        document.getElementById(alarmId).className = stepper.isAlarmTriggered ? "active-red-dot" : "inactive-red-dot";

        // gpios
        let gpioDisableId = `id_stepper_gpio_disable_${stepperIndex}`;
        let gpioForwardId = `id_stepper_gpio_forward_${stepperIndex}`;
        let gpioClockId = `id_stepper_gpio_clock_${stepperIndex}`;
        document.getElementById(gpioDisableId).checked = stepper.gpios.isDisableHigh;
        document.getElementById(gpioForwardId).checked = stepper.gpios.isForwardHigh;
        document.getElementById(gpioClockId).checked = stepper.gpios.isClockHigh;

        // timer
        document.getElementById(`id_stepper_timer_save_${stepperIndex}`).disabled = 
            (document.getElementById(`id_stepper_timer_select_${stepperIndex}`).value == stepper.config.timer)

        // encoder
        document.getElementById(`id_stepper_encoder_save_${stepperIndex}`).disabled = 
            (document.getElementById(`id_stepper_encoder_select_${stepperIndex}`).value == stepper.config.encoder);
        if(stepper.config.encoder == "NOT_SELECTED")
        {
            document.getElementById(`id_stepper_encoder_offset_${stepperIndex}`).textContent = "";
            document.getElementById(`id_stepper_encoder_maxOffsetError_${stepperIndex}`).textContent = "";
        }
        else
        {
            document.getElementById(`id_stepper_encoder_offset_${stepperIndex}`).textContent = ` Offset: ${stepper.status.encoderOffset}`;
            document.getElementById(`id_stepper_encoder_maxOffsetError_${stepperIndex}`).textContent = ` MaxOffsetError: ${stepper.status.maxEncoderOffsetError}`;
        }

        // forced mode
        document.getElementById(`id_stepper_period_forced_save_${stepperIndex}`).disabled = 
            (document.getElementById(`id_stepper_period_forced_config_${stepperIndex}`).value == stepper.config.forcedModeConfig.pulseWidth);

        // active mode
        document.getElementById(`id_stepper_period_active_starting_save_${stepperIndex}`).disabled = 
            (document.getElementById(`id_stepper_period_active_starting_config_${stepperIndex}`).value == stepper.config.activeModeConfig.startingPulseWidth);
        document.getElementById(`id_stepper_period_active_accelerationSteps_save_${stepperIndex}`).disabled =
            (document.getElementById(`id_stepper_period_active_accelerationSteps_config_${stepperIndex}`).value == stepper.config.activeModeConfig.acceleratingSteps);
        document.getElementById(`id_stepper_period_active_cruising_save_${stepperIndex}`).disabled =
            (document.getElementById(`id_stepper_period_active_cruising_config_${stepperIndex}`).value == stepper.config.activeModeConfig.cruisingPulseWidth);
        document.getElementById(`id_stepper_period_active_ending_save_${stepperIndex}`).disabled =
            (document.getElementById(`id_stepper_period_active_ending_config_${stepperIndex}`).value == stepper.config.activeModeConfig.endingPulseWidth);
        document.getElementById(`id_stepper_period_active_deaccelerationSteps_save_${stepperIndex}`).disabled =
            (document.getElementById(`id_stepper_period_active_deaccelerationSteps_config_${stepperIndex}`).value == stepper.config.activeModeConfig.deacceleratingSteps);

        // isEnableHigh
        let isEnableHighValueId = `id_stepper_isEnableHigh_value_${stepperIndex}`;
        let isEnableHighSaveId = `id_stepper_isEnableHigh_save_${stepperIndex}`;
        if(document.getElementById(isEnableHighValueId).checked == stepper.config.isEnableHigh)
        {
            document.getElementById(isEnableHighSaveId).disabled = true;
        }
        else
        {
            document.getElementById(isEnableHighSaveId).disabled = false;
        }
        // isForwardHigh
        let isForwardHighValueId = `id_stepper_isForwardHigh_value_${stepperIndex}`;
        let isForwardHighSaveId = `id_stepper_isForwardHigh_save_${stepperIndex}`;
        if(document.getElementById(isForwardHighValueId).checked == stepper.config.isForwardHigh)
        {
            document.getElementById(isForwardHighSaveId).disabled = true;
        }
        else
        {
            document.getElementById(isForwardHighSaveId).disabled = false;
        }
        // isRisingEdgeDriven
        let isRisingEdgeDrivenValueId = `id_stepper_isRisingEdgeDriven_value_${stepperIndex}`;
        let isRisingEdgeDrivenSaveId = `id_stepper_isRisingEdgeDriven_save_${stepperIndex}`;
        if(document.getElementById(isRisingEdgeDrivenValueId).checked == stepper.config.isRisingEdgeDriven)
        {
            document.getElementById(isRisingEdgeDrivenSaveId).disabled = true;
        }
        else
        {
            document.getElementById(isRisingEdgeDrivenSaveId).disabled = false;
        }

        // homeBoundaryToReadySteps
        let homeBoundaryToReadyStepsValueId = `id_stepper_homeBoundaryToReadySteps_value_${stepperIndex}`;
        let homeBoundaryToReadyStepsSaveId = `id_stepper_homeBoundaryToReadySteps_save_${stepperIndex}`;
        var homeBoundaryToReadySteps = document.getElementById(homeBoundaryToReadyStepsValueId).valueAsNumber
        if(Number.isNaN(homeBoundaryToReadySteps))
        {
            document.getElementById(homeBoundaryToReadyStepsValueId).valueAsNumber = stepper.config.homeBoundaryToReadySteps;
            homeBoundaryToReadySteps = stepper.config.homeBoundaryToReadySteps;
        }
        if(homeBoundaryToReadySteps == stepper.config.homeBoundaryToReadySteps)
        {
            document.getElementById(homeBoundaryToReadyStepsSaveId).disabled = true;
        }
        else
        {
            document.getElementById(homeBoundaryToReadyStepsSaveId).disabled = false;
        }

        // range
        let rangeValueId = `id_stepper_range_value_${stepperIndex}`;
        let rangeSaveId = `id_stepper_range_save_${stepperIndex}`;
        var range = document.getElementById(rangeValueId).valueAsNumber
        if(Number.isNaN(range))
        {
            document.getElementById(rangeValueId).valueAsNumber = stepper.config.range;
            range = stepper.config.range;
        }
        if(range == stepper.config.range)
        {
            document.getElementById(rangeSaveId).disabled = true;
        }
        else
        {
            document.getElementById(rangeSaveId).disabled = false;
        }

        // stepsPerRotation
        let stepsPerRotationValueId = `id_stepper_stepsPerRotation_value_${stepperIndex}`;
        let stepsPerRotationSaveId = `id_stepper_stepsPerRotation_save_${stepperIndex}`;
        var stepsPerRotation = document.getElementById(stepsPerRotationValueId).valueAsNumber
        if(Number.isNaN(stepsPerRotation))
        {
            document.getElementById(stepsPerRotationValueId).valueAsNumber = stepper.config.stepsPerRotation;
            stepsPerRotation = stepper.config.stepsPerRotation;
        }
        if(stepsPerRotation == stepper.config.stepsPerRotation)
        {
            document.getElementById(stepsPerRotationSaveId).disabled = true;
        }
        else
        {
            document.getElementById(stepsPerRotationSaveId).disabled = false;
        }

        // encoderCountsPerRotation
        let encoderCountsPerRotationValueId = `id_stepper_encoderCountsPerRotation_value_${stepperIndex}`;
        let encoderCountsPerRotationSaveId = `id_stepper_encoderCountsPerRotation_save_${stepperIndex}`;
        var encoderCountsPerRotation = document.getElementById(encoderCountsPerRotationValueId).valueAsNumber
        if(Number.isNaN(encoderCountsPerRotation))
        {
            document.getElementById(encoderCountsPerRotationValueId).valueAsNumber = stepper.config.encoderCountsPerRotation;
            encoderCountsPerRotation = stepper.config.encoderCountsPerRotation;
        }
        if(encoderCountsPerRotation == stepper.config.encoderCountsPerRotation)
        {
            document.getElementById(encoderCountsPerRotationSaveId).disabled = true;
        }
        else
        {
            document.getElementById(encoderCountsPerRotationSaveId).disabled = false;
        }

        // encoderOffsetErrorThreshold
        let encoderOffsetErrorThresholdValueId = `id_stepper_encoderOffsetErrorThreshold_value_${stepperIndex}`;
        let encoderOffsetErrorThresholdSaveId = `id_stepper_encoderOffsetErrorThreshold_save_${stepperIndex}`;
        var encoderOffsetErrorThreshold = document.getElementById(encoderOffsetErrorThresholdValueId).valueAsNumber
        if(Number.isNaN(encoderOffsetErrorThreshold))
        {
            document.getElementById(encoderOffsetErrorThresholdValueId).valueAsNumber = stepper.config.encoderOffsetErrorThreshold;
            encoderOffsetErrorThreshold = stepper.config.encoderOffsetErrorThreshold;
        }
        if(encoderOffsetErrorThreshold == stepper.config.encoderOffsetErrorThreshold)
        {
            document.getElementById(encoderOffsetErrorThresholdSaveId).disabled = true;
        }
        else
        {
            document.getElementById(encoderOffsetErrorThresholdSaveId).disabled = false;
        }

        // PortHomeBoundary
        let portHomeBoundarySelectId = `id_stepper_port_HomeBoundary_select_${stepperIndex}`;
        let portHomeBoundarySaveId = `id_stepper_port_HomeBoundary_save_${stepperIndex}`;
        let pinHomeBoundarySelectId = `id_stepper_pin_HomeBoundary_select_${stepperIndex}`;
        let pinHomeBoundarySaveId = `id_stepper_pin_HomeBoundary_save_${stepperIndex}`;
        if(document.getElementById(portHomeBoundarySelectId).value == "")
        {
            document.getElementById(portHomeBoundarySelectId).value = stepper.config.portHomeBoundary;
        }
        if(document.getElementById(portHomeBoundarySelectId).value == stepper.config.portHomeBoundary)
        {
            document.getElementById(portHomeBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(portHomeBoundarySaveId).disabled = false;
        }
        if(document.getElementById(pinHomeBoundarySelectId).value == "")
        {
            document.getElementById(pinHomeBoundarySelectId).value = stepper.config.pinHomeBoundary;
        }
        if(document.getElementById(pinHomeBoundarySelectId).value == stepper.config.pinHomeBoundary)
        {
            document.getElementById(pinHomeBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinHomeBoundarySaveId).disabled = false;
        }

        // PortEndBoundary
        let portEndBoundarySelectId = `id_stepper_port_EndBoundary_select_${stepperIndex}`;
        let portEndBoundarySaveId = `id_stepper_port_EndBoundary_save_${stepperIndex}`;
        let pinEndBoundarySelectId = `id_stepper_pin_EndBoundary_select_${stepperIndex}`;
        let pinEndBoundarySaveId = `id_stepper_pin_EndBoundary_save_${stepperIndex}`;
        if(document.getElementById(portEndBoundarySelectId).value == "")
        {
            document.getElementById(portEndBoundarySelectId).value = stepper.config.portEndBoundary;
        }
        if(document.getElementById(portEndBoundarySelectId).value == stepper.config.portEndBoundary)
        {
            document.getElementById(portEndBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(portEndBoundarySaveId).disabled = false;
        }
        if(document.getElementById(pinEndBoundarySelectId).value == "")
        {
            document.getElementById(pinEndBoundarySelectId).value = stepper.config.pinEndBoundary;
        }
        if(document.getElementById(pinEndBoundarySelectId).value == stepper.config.pinEndBoundary)
        {
            document.getElementById(pinEndBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinEndBoundarySaveId).disabled = false;
        }

        // PortEnable
        let portEnableSelectId = `id_stepper_port_Enable_select_${stepperIndex}`;
        let portEnableSaveId = `id_stepper_port_Enable_save_${stepperIndex}`;
        let pinEnableSelectId = `id_stepper_pin_Enable_select_${stepperIndex}`;
        let pinEnableSaveId = `id_stepper_pin_Enable_save_${stepperIndex}`;
        if(document.getElementById(portEnableSelectId).value == "")
        {
            document.getElementById(portEnableSelectId).value = stepper.config.portEnable;
        }
        if(document.getElementById(portEnableSelectId).value == stepper.config.portEnable)
        {
            document.getElementById(portEnableSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portEnableSaveId).disabled = false;
        }
        if(document.getElementById(pinEnableSelectId).value == "")
        {
            document.getElementById(pinEnableSelectId).value = stepper.config.pinEnable;
        }
        if(document.getElementById(pinEnableSelectId).value == stepper.config.pinEnable)
        {
            document.getElementById(pinEnableSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinEnableSaveId).disabled = false;
        }

        // PortForward 
        let portForwardSelectId = `id_stepper_port_Forward_select_${stepperIndex}`;
        let portForwardSaveId = `id_stepper_port_Forward_save_${stepperIndex}`;
        let pinForwardSelectId = `id_stepper_pin_Forward_select_${stepperIndex}`;
        let pinForwardSaveId = `id_stepper_pin_Forward_save_${stepperIndex}`;
        if(document.getElementById(portForwardSelectId).value == "")
        {
            document.getElementById(portForwardSelectId).value = stepper.config.portForward;
        }
        if(document.getElementById(portForwardSelectId).value == stepper.config.portForward)
        {
            document.getElementById(portForwardSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portForwardSaveId).disabled = false;
        }
        if(document.getElementById(pinForwardSelectId).value == "")
        {
            document.getElementById(pinForwardSelectId).value = stepper.config.pinForward;
        }
        if(document.getElementById(pinForwardSelectId).value == stepper.config.pinForward)
        {
            document.getElementById(pinForwardSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinForwardSaveId).disabled = false;
        }

        // PortClock 
        let portClockSelectId = `id_stepper_port_Clock_select_${stepperIndex}`;
        let portClockSaveId = `id_stepper_port_Clock_save_${stepperIndex}`;
        let pinClockSelectId = `id_stepper_pin_Clock_select_${stepperIndex}`;
        let pinClockSaveId = `id_stepper_pin_Clock_save_${stepperIndex}`;
        if(document.getElementById(portClockSelectId).value == "")
        {
            document.getElementById(portClockSelectId).value = stepper.config.portClock;
        }
        if(document.getElementById(portClockSelectId).value == stepper.config.portClock)
        {
            document.getElementById(portClockSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portClockSaveId).disabled = false;
        }
        if(document.getElementById(pinClockSelectId).value == "")
        {
            document.getElementById(pinClockSelectId).value = stepper.config.pinClock;
        }
        if(document.getElementById(pinClockSelectId).value == stepper.config.pinClock)
        {
            document.getElementById(pinClockSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinClockSaveId).disabled = false;
        }

        //  crossBoundary
        let crossBoundaryEnableId = `id_stepper_crossBoundary_enable_${stepperIndex}`;
        let crossBoundarySaveId = `id_stepper_crossBoundary_save_${stepperIndex}`;
        let crossBoundaryEnable = document.getElementById(crossBoundaryEnableId);
        let negativeRangeLabel = document.getElementById(`id_stepper_crossBoundary_negativeRangeLabel_${stepperIndex}`);
        let negativeRangeValue = document.getElementById(`id_stepper_crossBoundary_negativeRangeValue_${stepperIndex}`);
        document.getElementById(crossBoundarySaveId).disabled = 
            (crossBoundaryEnable.checked == stepper.config.crossBoundary.enabled) &&
            (Number(negativeRangeValue.value) == stepper.config.crossBoundary.negativeRange);
        if(crossBoundaryEnable.checked ) 
        {
            negativeRangeLabel.className = "";
            negativeRangeValue.disabled = false;
        }
        else
        {
            negativeRangeLabel.className = "disabled-label";
            negativeRangeValue.disabled = true;
        }
        // crossBoundaryItems
        for(let boundaryIndex = 0; boundaryIndex < stepper.config.crossBoundary.boundaries.length; boundaryIndex++)
        {
            let boundaryConfig = stepper.config.crossBoundary.boundaries[boundaryIndex];

            let boundaryEnable = document.getElementById(`id_stepper_crossBoundaryItem_enable_${boundaryIndex}_${stepperIndex}`);
            let boundaryValue = document.getElementById(`id_stepper_crossBoundaryItem_value_${boundaryIndex}_${stepperIndex}`);
            let boundaryError = document.getElementById(`id_stepper_crossBoundaryItem_error_${boundaryIndex}_${stepperIndex}`);
            let boundarySave = document.getElementById(`id_stepper_crossBoundaryItem_save_${boundaryIndex}_${stepperIndex}`);
            let boundaryLabel = boundaryEnable.closest("label");
            let boundaryErrorLabel = document.getElementById(`id_stepper_crossBoundaryItem_errorLabel_${boundaryIndex}_${stepperIndex}`);

            if(crossBoundaryEnable.checked == true)
            {
                boundaryLabel.className = "";
                boundaryEnable.disabled = false;

                if(boundaryEnable.checked) 
                {
                    boundaryValue.disabled = false;
                    boundaryErrorLabel.className = "";
                    boundaryError.disabled = false;
                }
                else 
                {
                    boundaryValue.disabled = true;
                    boundaryErrorLabel.className = "disabled-label";
                    boundaryError.disabled = true;
                }
                
                boundarySave.disabled = 
                    (boundaryEnable.checked == boundaryConfig.enabled) &&
                    (Number(boundaryValue.value) == boundaryConfig.value) &&
                    (Number(boundaryError.value) == boundaryConfig.error);
            }
            else
            {
                // disable all boundaries.
                boundaryLabel.className = "disabled-label";
                boundaryEnable.disabled = true;
                boundaryValue.disabled = true;
                boundaryErrorLabel.className = "disabled-label";
                boundaryError.disabled = true;
                boundarySave.disabled = true;
            }
        }

        document.getElementById(`id_stepper_control_enable_${stepperIndex}`).checked = stepper.status.isEnabled;
        document.getElementById(`id_stepper_control_forward_${stepperIndex}`).checked = stepper.status.isForward;

        document.getElementById(`id_stepper_test_enable_${stepperIndex}`).checked = stepper.status.isEnabled;
        document.getElementById(`id_stepper_test_forward_${stepperIndex}`).checked = stepper.status.isForward;
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

initConfigWidgets();

document.addEventListener('click', async function(event) { onClick(event); } );
document.addEventListener('input', (e) => {
    const t = e.target;
    if (t instanceof HTMLInputElement && t.type === 'number') {
        const id = t.id;                  // ID of the input
        onInputNumber(id);
    }
});

let intervalId = setInterval(updateUI, 1000);
