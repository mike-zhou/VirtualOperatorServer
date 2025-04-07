
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

document.addEventListener('click', async function(event) {
    onDocumentClick(event);
} );

