
async function getVersion() {
    try {
        document.getElementById("id_versionResult").innerHTML = "";
        let requestStr = `/get/Version`;
        let response = await fetch(requestStr);
        let data = await response.text();
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
            
            let requestStr = `/post/Echo`;
            let response = await fetch(requestStr, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            let data = await response.text();
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
        let requestStr = `/get/GPIOMode`;
        let response = await fetch(requestStr);
        let data = await response.text();
        document.getElementById("id_gpioModeResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function readGPIO() {
    try {
        document.getElementById("id_readGpioResult").innerHTML = "";
        let requestStr = `/get/GPIO`;
        let response = await fetch(requestStr);
        let data = await response.text();
        document.getElementById("id_readGpioResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function onDocumentClick(event)
{
    const element = event.target;
    const targetWithId = element.closest('[id]');

    switch(targetWithId.id)
    {
        case 'id_getVersion':
            await getVersion();
            break;
        case 'id_testEcho':
            await testEcho();
            break;
        case 'id_getGpioMode':
            await getGPIOMode();
            break;
        case 'id_readGpio':
            await readGPIO();
            break;

        default:
            break;
    }
}

document.addEventListener('click', async function(event) {
    onDocumentClick(event);
} );

