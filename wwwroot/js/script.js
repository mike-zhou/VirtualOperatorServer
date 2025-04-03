
async function getVersion() {
    try {
        document.getElementById("versionResult").innerHTML = "";
        let requestStr = `/get/Version`;
        let response = await fetch(requestStr);
        let data = await response.text();
        document.getElementById("versionResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function testEcho() {
    try {
        let echoCount = parseInt(document.getElementById("echoCount").value);
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

            document.getElementById("echoProgress").value = i * 100 / echoCount;
        }
        document.getElementById("echoProgress").value = 100;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function getGPIOMode()
{
    try {
        document.getElementById("gpioModeResult").innerHTML = "";
        let requestStr = `/get/GPIOMode`;
        let response = await fetch(requestStr);
        let data = await response.text();
        document.getElementById("gpioModeResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}

async function getGPIO() {
    try {
        document.getElementById("getGpioResult").innerHTML = "";
        let requestStr = `/get/GPIO`;
        let response = await fetch(requestStr);
        let data = await response.text();
        document.getElementById("getGpioResult").innerHTML = data;
    } catch (error) {
        console.error("Error:", error);
    }
}
