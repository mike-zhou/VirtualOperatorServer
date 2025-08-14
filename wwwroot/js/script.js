
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
        html.push(`<label id="id_stepper_period_forced_name_${stepperIndex}">Pulse period: </label>`);
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

        html.push(`<label id="id_stepper_period_active_starting_name_${stepperIndex}">Starting pulse period: </label>`);
        html.push(`<label id="id_stepper_period_active_starting_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_starting_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_starting_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_starting_save_${stepperIndex}" value="Save">`);
        html.push(`<label id="id_stepper_period_active_accelerationSteps_name_${stepperIndex}">Acceleration steps: </label>`);
        html.push(`<label id="id_stepper_period_active_accelerationSteps_value_${stepperIndex}"></label>`);
        html.push(`<input type="numbe\" id="id_stepper_period_active_accelerationSteps_config_${stepperIndex}" min="1" step="1" max="1024">`);
        html.push(`<input type="button" id="id_stepper_period_active_accelerationSteps_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_accelerationSteps_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label id="id_stepper_period_active_cruising_name_${stepperIndex}">Cruising period: </label>`);
        html.push(`<label id="id_stepper_period_active_cruising_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_cruising_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_cruising_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_cruising_save_${stepperIndex}" value="Save">`);
        html.push("<br>");

        html.push(`<label id="id_stepper_period_active_ending_name_${stepperIndex}">Ending pulse period: </label>`);
        html.push(`<label id="id_stepper_period_active_ending_value_${stepperIndex}"></label>`);
        html.push(`<input type="number" id="id_stepper_period_active_ending_config_${stepperIndex}" min="1" step="1" max="65536">`);
        html.push(`<input type="button" id="id_stepper_period_active_ending_set_${stepperIndex}" value="Set">`);
        html.push(`<input type="button" id="id_stepper_period_active_ending_save_${stepperIndex}" value="Save">`);
        html.push(`<label id="id_stepper_period_active_deaccelerationSteps_name_${stepperIndex}">Deacceleration steps: </label>`);
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
            html.push(`<label id='id_stepper_timer_value_${stepperIndex}'></label>`)
            html.push(`<select id="id_stepper_timer_select_${stepperIndex}">`);
            for(let i=0; i<timerOptionList.length - 1; i++)
            {
                html.push(`<option value="${timerOptionList[i]}">${timerOptionList[i]}</option>`);
            }
            html.push(`<option value="${timerOptionList.at(-1)}" selected>${timerOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_timer_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_timer_save_${stepperIndex}" value="Save"></td>`);
            html.push("</div>");
            // encoder
            html.push("<div>");
            html.push(`<label>Encoder: </label>`);
            html.push(`<label id='id_stepper_encoder_value_${stepperIndex}'></label>`);
            html.push(`<select id="id_stepper_encoder_select_${stepperIndex}">`);
            for(let i=0; i<encoderOptionList.length - 1; i++)
            {
                html.push(`<option value="${encoderOptionList[i]}">${encoderOptionList[i]}</option>`);
            }
            html.push(`<option value="${encoderOptionList.at(-1)}" selected>${encoderOptionList.at(-1)}</option>`);
            html.push("</select>");
            html.push(`<td><input type="button" id="id_stepper_encoder_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_encoder_save_${stepperIndex}" value="Save"></td>`);
            html.push("</div>");
            // mode
            html.push(createStepperMode(stepperIndex));
            // isEnableHigh, IsForwardHigh, IsRisingEdgeDriven
            html.push("<div><table>");
            html.push(`<tr>`);
            html.push(`<td><label>isEnableHigh<input type="checkbox" id="id_stepper_isEnableHigh_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isEnableHigh_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_isEnableHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsForwardHigh<input type="checkbox" id="id_stepper_isForwardHigh_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isForwardHigh_set_${stepperIndex}" value="Set"></td>`);
            html.push(`<td><input type="button" id="id_stepper_isForwardHigh_save_${stepperIndex}" value="Save"></td>`);
            html.push(`</tr>`);
            html.push(`<tr>`);
            html.push(`<td><label>IsRisingEdgeHigh<input type="checkbox" id="id_stepper_isRisingEdgeDriven_value_${stepperIndex}"></label></td>`);
            html.push(`<td><input type="button" id="id_stepper_isRisingEdgeDriven_set_${stepperIndex}" value="Set"></td>`);
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

async function onClick_PowerOutput(id) 
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

async function onClick_BDCPowerOutput(id) 
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

async function onClick_BDCControl(id) 
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
}

async function onClick_Stepper(id) 
{
    let segments = id.split("_");
    let classification = segments[2];

    if(classification == "gpio")
    {
        let stepperId = segments[4];

        if(action == "disable")
        {
            payload = {
                index: parseInt(stepperId, 10),
                disableStepper: document.getElementById(id).checked
            }

            data = await post('disableStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to enabl stepper ${stepperId}, info: ${data}`);
            }
        }
        else if(action == "forward")
        {
            payload = {
                index: parseInt(stepperId, 10),
                forwardStepper: document.getElementById(id).checked
            }

            data = await post('forwardStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to forward stepper ${stepperId}, info: ${data}`);
            }
        }
        else if(action == "clock")
        {
            payload = {
                index: parseInt(stepperId, 10),
                highLevel: document.getElementById(id).checked
            }

            data = await post('clockStepper', payload);
            if(data != "success")
            {
                alert(`Error: failed to clock stepper ${stepperId}, info: ${data}`);
            }
        }
    }
    else if(classification == "timer")
    {
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
                alert(`${errString}, info: ${data}`);
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
                alert(`${errString}, info: ${data}`);
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
                alert(`${errString}, info: ${data}`);
            }
        }
    }
    else if(classification == "control")
    {
        // do nothing
    }
    else if(classification == "steps")
    {
        // do nothing
    }
    else if(classification == "go")
    {
        let stepperIndex = parseInt(segments[4], 10);
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
            alert(`Error: Out of range of steps: ${stepsNum}`);
            return;
        }

        var mode = "";

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

        var forward = document.getElementById(`id_stepper_control_forward_${stepperIndex}`).checked;

        payload = {
            stepperId: Number(stepperIndex),
            mode: mode,
            forward: forward,
            steps: stepsNum
        }

        if(mode == "passive")
        {
            var activeStepper = document.getElementById(`id_stepper_passive_activeStepper_select_${stepperIndex}`).value;
            
            if(activeStepper == "NOT_SELECTED")
            {
                alert(`Error: active stepper is not selected in passive mode`);
                return;
            }

            payload.activeStepperId = Number(activeStepper.split('_')[1]);
        }

        data = await post('runStepper', payload);
        if(data != "success")
        {
            alert(`Error: failed to run stepper ${segments[4]}, info: ${data}`);
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
    else if (elementId.startsWith("id_flexTimer_prescaler_"))
        await onClick_FlexTimerPrescaler(elementId);
    else if (elementId.startsWith("id_fixTimer_prescaler_"))
        await onClick_FixTimerPrescaler(elementId);
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

        document.getElementById(unitId).textContent = (status.flexTimers[i].prescaler * 4.16).toLocaleString('en-US'); 
    }

    document.getElementById("id_fixTimer_state").textContent = status.fixTimer.state;
    document.getElementById("id_fixTimer_prescaler_value").textContent = String(status.fixTimer.prescaler);
    if(document.getElementById("id_fixTimer_prescaler_config").value === "")
    {
        document.getElementById("id_fixTimer_prescaler_config").value = status.fixTimer.prescalerConfig;
    }

    document.getElementById("id_fixTimer_prescaler_unit").textContent = (status.fixTimer.prescaler * 10).toLocaleString('en-US'); 
}

function updateStepper(status)
{
    for(let stepperIndex=0; stepperIndex<status.steppers.length; stepperIndex++)
    {
        let data = status.steppers[stepperIndex];

        // alarm
        let alarmId = `id_stepperAlarm_state_${stepperIndex}`;
        document.getElementById(alarmId).className = data.isAlarmTriggered ? "active-red-dot" : "inactive-red-dot";

        // gpios
        let gpioDisableId = `id_stepper_gpio_disable_${stepperIndex}`;
        let gpioForwardId = `id_stepper_gpio_forward_${stepperIndex}`;
        let gpioClockId = `id_stepper_gpio_clock_${stepperIndex}`;
        document.getElementById(gpioDisableId).checked = data.gpios.isDisableHigh;
        document.getElementById(gpioForwardId).checked = data.gpios.isForwardHigh;
        document.getElementById(gpioClockId).checked = data.gpios.isClockHigh;

        // timer
        let timerId = `id_stepper_timer_value_${stepperIndex}`;
        document.getElementById(timerId).textContent = data.config.timer;

        // encoder
        let encoderId = `id_stepper_encoder_value_${stepperIndex}`;
        document.getElementById(encoderId).textContent = data.config.encoder;

        // forced mode
        let forcedModePeriodValueId = `id_stepper_period_forced_value_${stepperIndex}`;
        document.getElementById(forcedModePeriodValueId).textContent = 
            String(data.config.forcedModeConfig.pulseWidth);

        // active mode
        let activeModeStartingPeriodValueId = `id_stepper_period_active_starting_value_${stepperIndex}`;
        let activeModeAccelerationStepsValueId = `id_stepper_period_active_accelerationSteps_value_${stepperIndex}`;
        let activeModeCruisePeriodValueId = `id_stepper_period_active_cruising_value_${stepperIndex}`;
        let activeModeEndingPeriodValueId = `id_stepper_period_active_ending_value_${stepperIndex}`;
        let activeModeDeaccelerationStepsValueId = `id_stepper_period_active_deaccelerationSteps_value_${stepperIndex}`;
        document.getElementById(activeModeStartingPeriodValueId).textContent =
            String(data.config.activeModeConfig.startingPulseWidth);
        document.getElementById(activeModeAccelerationStepsValueId).textContent =
            String(data.config.activeModeConfig.acceleratingSteps);
        document.getElementById(activeModeCruisePeriodValueId).textContent =
            String(data.config.activeModeConfig.cruisingPulseWidth);
        document.getElementById(activeModeEndingPeriodValueId).textContent =
            String(data.config.activeModeConfig.endingPulseWidth);
        document.getElementById(activeModeDeaccelerationStepsValueId).textContent =
            String(data.config.activeModeConfig.deacceleratingSteps);

        // isEnableHigh
        let isEnableHighValueId = `id_stepper_isEnableHigh_value_${stepperIndex}`;
        let isEnableHighSaveId = `id_stepper_isEnableHigh_save_${stepperIndex}`;
        if(document.getElementById(isEnableHighValueId).checked == data.config.isEnableHigh)
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
        if(document.getElementById(isForwardHighValueId).checked == data.config.isForwardHigh)
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
        if(document.getElementById(isRisingEdgeDrivenValueId).checked == data.config.isRisingEdgeDriven)
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
            document.getElementById(homeBoundaryToReadyStepsValueId).valueAsNumber = data.config.homeBoundaryToReadySteps;
            homeBoundaryToReadySteps = data.config.homeBoundaryToReadySteps;
        }
        if(homeBoundaryToReadySteps == data.config.homeBoundaryToReadySteps)
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
            document.getElementById(rangeValueId).valueAsNumber = data.config.range;
            range = data.config.range;
        }
        if(range == data.config.range)
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
            document.getElementById(stepsPerRotationValueId).valueAsNumber = data.config.stepsPerRotation;
            stepsPerRotation = data.config.stepsPerRotation;
        }
        if(stepsPerRotation == data.config.stepsPerRotation)
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
            document.getElementById(encoderCountsPerRotationValueId).valueAsNumber = data.config.encoderCountsPerRotation;
            encoderCountsPerRotation = data.config.encoderCountsPerRotation;
        }
        if(encoderCountsPerRotation == data.config.encoderCountsPerRotation)
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
            document.getElementById(encoderOffsetErrorThresholdValueId).valueAsNumber = data.config.encoderOffsetErrorThreshold;
            encoderOffsetErrorThreshold = data.config.encoderOffsetErrorThreshold;
        }
        if(encoderOffsetErrorThreshold == data.config.encoderOffsetErrorThreshold)
        {
            document.getElementById(encoderOffsetErrorThresholdSaveId).disabled = true;
        }
        else
        {
            document.getElementById(encoderOffsetErrorThresholdSaveId).disabled = false;
        }

        // HomeBoundary
        let portHomeBoundarySelectId = `id_stepper_port_HomeBoundary_select_${stepperIndex}`;
        let portHomeBoundarySaveId = `id_stepper_port_HomeBoundary_save_${stepperIndex}`;
        let pinHomeBoundarySelectId = `id_stepper_pin_HomeBoundary_select_${stepperIndex}`;
        let pinHomeBoundarySaveId = `id_stepper_pin_HomeBoundary_save_${stepperIndex}`;
        if(document.getElementById(portHomeBoundarySelectId).value == "")
        {
            document.getElementById(portHomeBoundarySelectId).value = data.config.portHomeBoundary;
        }
        if(document.getElementById(portHomeBoundarySelectId).value == data.config.portHomeBoundary)
        {
            document.getElementById(portHomeBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(portHomeBoundarySaveId).disabled = false;
        }
        if(document.getElementById(pinHomeBoundarySelectId).value == "")
        {
            document.getElementById(pinHomeBoundarySelectId).value = data.config.pinHomeBoundary;
        }
        if(document.getElementById(pinHomeBoundarySelectId).value == data.config.pinHomeBoundary)
        {
            document.getElementById(pinHomeBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinHomeBoundarySaveId).disabled = false;
        }

        // EndBoundary
        let portEndBoundarySelectId = `id_stepper_port_EndBoundary_select_${stepperIndex}`;
        let portEndBoundarySaveId = `id_stepper_port_EndBoundary_save_${stepperIndex}`;
        let pinEndBoundarySelectId = `id_stepper_pin_EndBoundary_select_${stepperIndex}`;
        let pinEndBoundarySaveId = `id_stepper_pin_EndBoundary_save_${stepperIndex}`;
        if(document.getElementById(portEndBoundarySelectId).value == "")
        {
            document.getElementById(portEndBoundarySelectId).value = data.config.portEndBoundary;
        }
        if(document.getElementById(portEndBoundarySelectId).value == data.config.portEndBoundary)
        {
            document.getElementById(portEndBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(portEndBoundarySaveId).disabled = false;
        }
        if(document.getElementById(pinEndBoundarySelectId).value == "")
        {
            document.getElementById(pinEndBoundarySelectId).value = data.config.pinEndBoundary;
        }
        if(document.getElementById(pinEndBoundarySelectId).value == data.config.pinEndBoundary)
        {
            document.getElementById(pinEndBoundarySaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinEndBoundarySaveId).disabled = false;
        }

        // Enable
        let portEnableSelectId = `id_stepper_port_Enable_select_${stepperIndex}`;
        let portEnableSaveId = `id_stepper_port_Enable_save_${stepperIndex}`;
        let pinEnableSelectId = `id_stepper_pin_Enable_select_${stepperIndex}`;
        let pinEnableSaveId = `id_stepper_pin_Enable_save_${stepperIndex}`;
        if(document.getElementById(portEnableSelectId).value == "")
        {
            document.getElementById(portEnableSelectId).value = data.config.portEnable;
        }
        if(document.getElementById(portEnableSelectId).value == data.config.portEnable)
        {
            document.getElementById(portEnableSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portEnableSaveId).disabled = false;
        }
        if(document.getElementById(pinEnableSelectId).value == "")
        {
            document.getElementById(pinEnableSelectId).value = data.config.pinEnable;
        }
        if(document.getElementById(pinEnableSelectId).value == data.config.pinEnable)
        {
            document.getElementById(pinEnableSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinEnableSaveId).disabled = false;
        }

        // Forward
        let portForwardSelectId = `id_stepper_port_Forward_select_${stepperIndex}`;
        let portForwardSaveId = `id_stepper_port_Forward_save_${stepperIndex}`;
        let pinForwardSelectId = `id_stepper_pin_Forward_select_${stepperIndex}`;
        let pinForwardSaveId = `id_stepper_pin_Forward_save_${stepperIndex}`;
        if(document.getElementById(portForwardSelectId).value == "")
        {
            document.getElementById(portForwardSelectId).value = data.config.portForward;
        }
        if(document.getElementById(portForwardSelectId).value == data.config.portForward)
        {
            document.getElementById(portForwardSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portForwardSaveId).disabled = false;
        }
        if(document.getElementById(pinForwardSelectId).value == "")
        {
            document.getElementById(pinForwardSelectId).value = data.config.pinForward;
        }
        if(document.getElementById(pinForwardSelectId).value == data.config.pinForward)
        {
            document.getElementById(pinForwardSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinForwardSaveId).disabled = false;
        }

        // Clock
        let portClockSelectId = `id_stepper_port_Clock_select_${stepperIndex}`;
        let portClockSaveId = `id_stepper_port_Clock_save_${stepperIndex}`;
        let pinClockSelectId = `id_stepper_pin_Clock_select_${stepperIndex}`;
        let pinClockSaveId = `id_stepper_pin_Clock_save_${stepperIndex}`;
        if(document.getElementById(portClockSelectId).value == "")
        {
            document.getElementById(portClockSelectId).value = data.config.portClock;
        }
        if(document.getElementById(portClockSelectId).value == data.config.portClock)
        {
            document.getElementById(portClockSaveId).disabled = true;
        }
        else
        {
            document.getElementById(portClockSaveId).disabled = false;
        }
        if(document.getElementById(pinClockSelectId).value == "")
        {
            document.getElementById(pinClockSelectId).value = data.config.pinClock;
        }
        if(document.getElementById(pinClockSelectId).value == data.config.pinClock)
        {
            document.getElementById(pinClockSaveId).disabled = true;
        }
        else
        {
            document.getElementById(pinClockSaveId).disabled = false;
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

document.addEventListener('click', async function(event) { onClick(event); } );
document.addEventListener('input', (e) => {
    const t = e.target;
    if (t instanceof HTMLInputElement && t.type === 'number') {
        const id = t.id;                  // ID of the input
        onInputNumber(id);
    }
});

let intervalId = setInterval(updateUI, 1000);

