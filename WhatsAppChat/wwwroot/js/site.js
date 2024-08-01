let receiver = 0;
let groupId = ``;
let isGroup = false;
let globalDay = 0;
let hrCounter = 0;

const messageBox = document.getElementById('messageBox');
const messages = document.getElementById('messages');
const currentTime = new Date();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => console.log('Connected!'))
    .catch(console.error);

connection.on('newMessage', async (sender, rec, messageText) => {
    if (rec == receiver && isGroup==false) {
        let response = await fetch(`${window.location.origin}/Services/markAsRead?senderId=${receiver}&receiverId=${Number(sender)}`);
        if (globalDay != -1) {
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 7px; background-color: lavender;">Today</span>
                </div>
            </div>`;
            globalDay = -1;
        }
        if (document.getElementById(`${receiver}:lastMessage`)) {
            document.getElementById(`${rec}:lastMessage`).innerText = messageText;
        }
        else {
            let userResponse = await fetch(`/Services/getSingleUser?id=${receiver}`);
            let userData = await userResponse.json();
            if (document.getElementById('users').innerHTML != '\n\t\t\t\t' && hrCounter != 0) {
                document.getElementById('users').innerHTML += `<hr />`;
            }
            hrCounter++;
            document.getElementById('users').innerHTML += `<div class="row userHover" style="align-items:center; margin:2px;" onclick="displayChat(${userData.id})">
                <div class="col-2 ps-4">
					<img src="${userData.profileImage.split('wwwroot')[1]}" style="border-radius: 100%" height="50px" width="50px">
				</div>
				<div class="col-8 ps-4">
					<b style="font-size:20px">${userData.userName}</b> <br />
					<span style="padding:5px; font-size:16px; opacity: 0.7;" id="${userData.id}:lastMessage">
						${messageText}
					</span>
				</div>
                <div class="col-2 pe-2">
                    <span style="border-radius:50%; background-color:green; color:white" id="${userData.id}"></span>
                </div>
            </div>`;
        }
        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2" style="align-items:end">
            <div class="col-10" style="overflow-wrap: anywhere;">
            <span style="background-color: rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">${messageText}</span>
            <br/>
            <span style="padding:5px; font-size:13px; opacity: 0.7;">
                ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
            </span>
            </div>
            <div class="col-2"></div>
        </div>`;
    }
    else {
        if (document.getElementById(`${rec}:lastMessage`)) {
            const numberEle = document.getElementById(`${rec}`);
            if (numberEle.innerText == '') {
                numberEle.style.padding = '2px 7px 4px 7px';
                numberEle.innerText = '1';

                // Display Last Message
                document.getElementById(`${rec}:lastMessage`).innerText = messageText;
            }
            else {
                numberEle.innerText = `${Number(numberEle.innerText) + 1}`;
                document.getElementById(`${rec}:lastMessage`).innerText = messageText;
            }
        }
        else {
            let userResponse = await fetch(`/Services/getSingleUser?id=${rec}`);
            let userData = await userResponse.json();
            if (document.getElementById('users').innerHTML != '\n\t\t\t\t' && hrCounter != 0) {
                document.getElementById('users').innerHTML += `<hr />`
            }
            hrCounter++;
            document.getElementById('users').innerHTML += `<div class="row userHover" style="align-items:center; margin:2px;" onclick="displayChat(${userData.id})">
                <div class="col-2 ps-4">
					<img src="${userData.profileImage.split('wwwroot')[1]}" style="border-radius: 100%" height="50px" width="50px">
				</div>
				<div class="col-8 ps-4">
					<b style="font-size:20px">${userData.userName}</b> <br />
					<span style="padding:5px; font-size:16px; opacity: 0.7;" id="${userData.id}:lastMessage">
						${messageText}
					</span>
				</div>
                <div class="col-2 pe-2">
                    <span style="border-radius:50%; background-color:green; color:white; padding:2px 7px 4px 7px" id="${userData.id}">1</span>
                </div>
            </div>`;
        }
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
});

connection.on('SendToGroup', async (senderId, senderName, group, message) => {
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    if (isGroup==true && groupId == group) {
        if (senderId.toString() != sender) {
            if (globalDay != -1) {
                messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 7px; background-color: lavender;">Today</span>
                </div>
            </div>`;
                globalDay = -1;
            }
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col-10 text-start" style="overflow-wrap: anywhere;">
                    <span style="background-color:rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">
                        ${message}
                    </span>
                    <br />
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)} : ${senderName}
                    </span>
                </div>
                <div class="col-2"></div>
            </div>`;
            document.getElementById(`${group}:LastMessage`).innerText = message;
        }
    }
    else {
        let response = await fetch(`/Services/AddInGroupUnreads?senderId=${sender}&groupId=${group}`);
        const numberEle = document.getElementById(`${group}`);
        if (numberEle.innerText == '') {
            numberEle.style.padding = '2px 7px 4px 7px';
            numberEle.innerText = '1';
            document.getElementById(`${group}:LastMessage`).innerText = message;
        }
        else {
            numberEle.innerText = `${Number(numberEle.innerText) + 1}`;
            document.getElementById(`${group}:LastMessage`).innerText = message;
        }
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
});

connection.on("ReceiveFile", async (sender, rec, urls) => {
    let fileCounter = 0;
    if (rec == receiver && isGroup == false) {
        let responseData = await fetch(`/Services/markAsRead?senderId=${rec}&receiverId=${Number(sender)}`);
        if (globalDay != -1) {
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 7px; background-color: lavender;">Today</span>
                </div>
            </div>`;
            globalDay = -1;
        }

        let htmlStr = ``;
        urls.urls.forEach((ele, index) => {
            htmlStr += `<div class="row ps-5 pe-5 pt-2 pb-2">
            <div class="col-10 text-start">`;
            if (urls.type[fileCounter].slice(0, 5) == "image") {
                htmlStr += `<a href="${ele}" target='_blank' download><img src="${ele}" height="300px" id="${ele}" /></a>`;
            }
            else {
                htmlStr += `<embed src="${ele}" height="300px" id="${ele}" />`;
            }
            document.getElementById(`${receiver}:lastMessage`).innerText = '🎞 File';
            htmlStr += `<br />
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                    </span>
                </div>
                <div class="col-2"></div>
            </div>`;
            fileCounter++;
        });
        messages.innerHTML += htmlStr;
        messages.scrollTop = messages.scrollHeight;
        messages.scrollIntoView();
    }
    else {
        const numberEle = document.getElementById(`${rec}`);
        if (numberEle.innerText == '') {
            numberEle.style.padding = '2px 7px 4px 7px';
            numberEle.innerText = `${urls.urls.length}`;
        }
        else {
            numberEle.innerText = `${Number(numberEle.innerText) + fileCounter}`;
        }

        // Display Last Message
        if (urls.type[urls.urls.length - 1].split("/")[0] == 'image') {
            document.getElementById(`${rec}:lastMessage`).innerText = `🎞 File`;
        }
        else {
            document.getElementById(`${rec}:lastMessage`).innerText = `🎞 File`;
        }
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
});

connection.on("ReceiveFromGroup", async (senderId, senderName, group, urls) => {
    let fileCounter = 0;
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    if (isGroup == true && groupId == group) {
        if (senderId.toString() != sender) {
            let messageString = ``;
            if (globalDay != -1) {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 7px; background-color: lavender;">Today</span>
                </div>
            </div>`;
                globalDay = -1;
            }
            urls.urls.forEach((ele, index) => {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-10 text-start" style="overflow-wrap: anywhere;">`;
                if (urls.type[fileCounter].slice(0, 5) == "image") {
                    messageString += `<a href="${ele}" target='_blank' download><img src="${ele}" height="300px" id="${ele}" /></a>`;
                }
                else {
                    messageString += `<embed src="${ele}" height="300px" id="${ele}" />`;
                    
                }
                document.getElementById(`${group}:LastMessage`).innerText = '🎞 File';
                messageString += `<br />
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)} : ${senderName}
                        </span>
                    </div>
                    <div class="col-2"></div>
                </div>`;
                fileCounter++;
            });
            messages.innerHTML += messageString;
        }
    }
    else {
        let response = await fetch(`/Services/AddInGroupUnreads?senderId=${sender}&groupId=${group}`);

        const numberEle = document.getElementById(`${group}`);
        if (numberEle.innerText == '') {
            numberEle.style.padding = '2px 7px 4px 7px';
            numberEle.innerText = `${urls.urls.length}`;
        }
        else {
            numberEle.innerText = `${Number(numberEle.innerText) + fileCounter}`;
        }

        // Display Last Message
        document.getElementById(`${group}:LastMessage`).innerText = `🎞 File`;
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
});

 async function send() {
    const message = messageBox.value;
    if (isGroup == false && receiver == 0) {
        alert('Please select the User or Group to chat with!');
    }
    else if (isGroup == true) {
        connection.invoke('SendToGroup', groupId, message);
        if (globalDay != -1) {
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                </div>
            </div>`;
            globalDay = -1;
        }
        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
            <div class="col-2"></div>
            <div class="col-10 text-end" style="overflow-wrap: anywhere;">
                <span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${message}</span>
                <br />
                <span style="padding:5px; font-size:13px; opacity: 0.7;">
                    ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                </span>
            </div>
        </div>`;
        messageBox.value = '';
        document.getElementById(`${groupId}:LastMessage`).innerText = message;
    }
    else {
        connection.invoke('SendMessage', receiver.toString(), message);
        if (globalDay != -1) {
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                </div>
            </div>`;
            globalDay = -1;
        }
        if(document.getElementById(`${receiver}:lastMessage`)) {
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col-2"></div>
                <div class="col-10 text-end" style="overflow-wrap: anywhere;">
                    <span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${message}</span>
                    <br />
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                    </span>
                </div>
            </div>`;
            document.getElementById(`${receiver}:lastMessage`).innerText = message;
            messageBox.value = '';
        }
        else {
            let userResponse = await fetch(`/Services/getSingleUser?id=${receiver}`);
            let userData = await userResponse.json();
            if (document.getElementById('users').innerHTML != '\n\t\t\t\t' && hrCounter != 0) {
                document.getElementById('users').innerHTML += `<hr />`
            }
            hrCounter++;
            document.getElementById('users').innerHTML += `<div class="row userHover" style="align-items:center; margin:2px;" onclick="displayChat(${userData.id})">
                <div class="col-2 ps-4">
					<img src="${userData.profileImage.split('wwwroot')[1]}" style="border-radius: 100%" height="50px" width="50px">
				</div>
				<div class="col-8 ps-4">
					<b style="font-size:20px">${userData.userName}</b> <br />
					<span style="padding:5px; font-size:16px; opacity: 0.7;" id="${userData.id}:lastMessage">
						${message}
					</span>
				</div>
                <div class="col-2 pe-2">
                    <span style="border-radius:50%; background-color:green; color:white" id="${userData.id}"></span>
                </div>
            </div>`;
            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col-2"></div>
                <div class="col-10 text-end" style="overflow-wrap: anywhere;">
                    <span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${message}</span>
                    <br />
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                    </span>
                </div>
            </div>`;
            messageBox.value = '';

        }
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
}

async function displayChat(id) {
    document.getElementById('searchSuggetion').innerHTML = '';
    document.getElementById('searchResult').value = '';

    let temp = new Date().toLocaleDateString();
    const currentDate = new Date(temp);
    isGroup = false;
    const numberEle = document.getElementById(`${id}`);
    if (numberEle) {
        numberEle.style.padding = '0';
        numberEle.innerText = '';
    }
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];

    // Mark every messages as read
    let responseData = await fetch(`/Services/markAsRead?senderId=${id}&receiverId=${Number(sender)}`);
    messages.innerHTML = '';
    receiver = id;

    // Get User Chat History
    let response = await fetch(`${window.location.origin}/Services/getUserChat?senderId=${sender}&receiverId=${id}`);
    let data = await response.json();
    document.getElementById('chatProf').src = data.profileImage;
    document.getElementById('chatName').innerText = data.userName;
    if (data.logoutTime == null) {
        document.getElementById('chatStatus').style.color = 'green';
        document.getElementById('chatStatus').innerText = 'Online';
    }
    else {
        document.getElementById('chatStatus').style.color = '#777777';
        document.getElementById('chatStatus').innerText = `Last Seen : ${data.logoutTime.slice(0, 10)} /  ${data.logoutTime.slice(11, 16)}`;
    }
    if (data.message != null) {
        let dateArr = [];
        let messageString = ``;
        data.message.forEach(function (ele, index) {
            var messageDate = new Date(Date.parse(ele.sendTime));
            let dateDiff = Math.floor((currentDate.getTime() - messageDate.getTime()) / 86400000);
            if (!dateArr.includes(dateDiff)) {
                dateArr.push(dateDiff);
                if (dateDiff == -1) {
                    messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center">
                            <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                        </div>
                    </div>`;
                }
                else if (dateDiff == 0) {
                    messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center" style="backgroud-color:yellow">
                            <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Yesterday</span>
                        </div>
                    </div>`;
                }
                else {
                    messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center" style="backgroud-color:yellow">
                            <span style="border-radius: 10px; padding: 3px; background-color: lavender;">${ele.sendTime.toString().slice(0, 10)}</span>
                        </div>
                    </div>`;
                }
            }
            if (ele.senderId.toString() == ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0]) {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-2"></div>
                    <div class="col-10 text-end" style="overflow-wrap: anywhere;">`;
                if (ele.filePath != null) {
                    if (ele.fileType.slice(0, 5) == 'image') {
                        messageString += `<a href="${ele.filePath}" target='_blank' download><img src="${ele.filePath}" height="300px" id="${ele.filePath}" /></a>`;
                    }
                    else {
                        messageString += `<embed src="${ele.filePath}" height="300px" id="${ele.filePath}" />`;
                    }
                }
                else if (ele.message != null) {
                    messageString += `<span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${ele.message}</span>`;
                }    
                messageString += `<br/>
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${ele.sendTime.toString().slice(11, 16)}
                    </span>
                    </div>
                </div>`;
                
            }
            else {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col-10 text-start" style="overflow-wrap: anywhere;">`;
                if (ele.filePath != null) {
                    if (ele.fileType.slice(0, 5) == 'image') {
                        messageString += `<a href="${ele.filePath}" target='_blank' download><img src="${ele.filePath}" height="300px" id="${ele.filePath}" /></a>`;
                    }
                    else {
                        messageString += `<embed src="${ele.filePath}" height="300px" id="${ele.filePath}" />`;
                    }
                }
                else if (ele.message != null) {
                    messageString += `<span style="background-color: rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">${ele.message}</span>`;
                }
                messageString += `<br/>
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)}
                        </span>
                    </div>
                    <div class="col-2"></div>
                </div>`;
            }
        });
        messages.innerHTML = messageString;
        globalDay = dateArr[dateArr.length - 1];
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
}

async function logOut() {
    connection.stop();
    window.location.href = window.location.origin + '/Login';
}

async function displayForm() {
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    let response = await fetch(`${window.location.origin}/Services/getAllUsers?id=${sender}`);
    let dataUsers = await response.json();
    let userString = `<select name="GroupMembers" class="form-control" multiple>`;
    dataUsers.forEach(function (ele, index) {
        userString += `<option value="${ele.id}">${ele.userName}</option>`
    });
    userString += `</select>`;

    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: 'Create Group',
        html: `<form enctype="multipart/form-data" action="/Services/CreateGroup" method="post">
            <label class="text-primary">Group Icon</label>
            <input type="file" class="form-control" accept=".jpg, .png, .jfif" name="GroupIcon" /><br />
            <input type="text" name="GroupName" class="form-control" placeholder="Group Name"/><br />
            ${userString}<br />
            <input type="submit" class="btn btn-primary" value="CREATE">
        </form>`,
        showConfirmButton: false
    });
}

async function displaygroupChat(id) {
    document.getElementById('searchSuggetion').innerHTML = '';
    document.getElementById('searchResult').value = '';

    isGroup = true;
    let temp = new Date().toLocaleDateString();
    const currentDate = new Date(temp);
    messages.innerHTML = ``;
    document.getElementById('chatStatus').innerText = ``;
    let response = await fetch(`${window.location.origin}/Services/GetGroupChat?id=${id}`);
    let data = await response.json();
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    groupId = data.id;
    document.getElementById(id).style.padding = 0;
    document.getElementById(id).innerText = '';
    document.getElementById('chatProf').src = data.groupIcon;
    document.getElementById('chatName').innerText = data.groupName;
    
    //Remove
    let groupResponse = await fetch(`Services/deleteFromUnreads?SenderId=${Number(sender)}&GroupId=${id}`);
    if (data.groupMembers != null) {
        let memberString = `You, `;
        data.groupMembers.forEach(function (ele, index) {
            if (ele.id != Number(sender)) {
                memberString += `${ele.userName}, `;
            }
        });
        memberString = memberString.slice(0, memberString.length - 2);
        document.getElementById('chatStatus').style.color = '#777777';
        document.getElementById('chatStatus').innerText = memberString
    }
    if (data.message != null) {
        let dateArr = [];
        data.message.forEach(function (ele, index) {
            var messageDate = new Date(Date.parse(ele.sendTime));
            let dateDiff = Math.floor((currentDate.getTime() - messageDate.getTime()) / 86400000);
            if (!dateArr.includes(dateDiff)) {
                dateArr.push(dateDiff);
                if (dateDiff == -1) {
                    messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center" style="backgroud-color:yellow">
                            <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                        </div>
                    </div>`;
                }
                else if (dateDiff == 0) {
                    messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center" style="backgroud-color:yellow">
                            <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Yesterday</span>
                        </div>
                    </div>`;
                }
                else {
                    messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center" style="backgroud-color:yellow">
                            <span style="border-radius: 10px; padding: 3px; background-color: lavender;">${ele.sendTime.toString().slice(0, 10)}</span>
                        </div>
                    </div>`;
                }
            }
            let messageString = ``;
            if (ele.senderId.toString() == sender) {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-2"></div>
                    <div class="col-10 text-end" style="overflow-wrap: anywhere;">`;
                if (ele.filePath != null) {
                    if (ele.fileType.slice(0, 5) == 'image') {
                        messageString += `<a href="${ele.filePath}" target='_blank' download><img src="${ele.filePath}" height="300px" id="${ele.filePath}" /></a>`;
                    }
                    else {
                        messageString += `<embed src="${ele.filePath}" height="300px" id="${ele.filePath}" />`;
                    }
                }
                else if (ele.message != null) {
                    messageString += `<span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${ele.message}</span>`;
                }
                messageString += `<br/>
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)}
                        </span>
                    </div>
                </div>`;
            }
            else {
                messageString += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-10 text-start" style="overflow-wrap: anywhere;">`;
                if (ele.filePath != null) {
                    if (ele.fileType.slice(0, 5) == 'image') {
                        messageString += `<a href="${ele.filePath}" target='_blank' download><img src="${ele.filePath}" height="300px" id="${ele.filePath}" /></a>`;
                    }
                    else {
                        messageString += `<embed src="${ele.filePath}" height="300px" id="${ele.filePath}" />`;
                    }
                }
                else if (ele.message != null) {
                    messageString += `<span style="background-color:rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">${ele.message}</span>`;
                }
                messageString += `<br />
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)} : ${ele.userName}
                        </span>
                    </div>
                    <div class="col-2"></div>
                </div>`;
            }
            messages.innerHTML += messageString;
            globalDay = dateArr[dateArr.length - 1];
        });
    }
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
}

function changeProfileImage() {
    const input = document.getElementById('userProfile');
    let fReader = new FileReader();
    fReader.readAsDataURL(input.files[0]);
    fReader.onload = function (event) {
        let img = document.getElementById('image');
        img.src = event.target.result;
    }
}

async function editProfile(id) {
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    let EditProfileRes = await fetch(`/Services/getSingleUser?id=${Number(sender)}`);
    let EditProfileData = await EditProfileRes.json();
    profImage = EditProfileData.profileImage.split('wwwroot/');
    Swal.fire({
        title: 'Edit Profile',
        html: `<form action="/Services/editProfile" method="post" enctype="multipart/form-data" style="text-align:left">
            <input type="number" name="Id" value="${EditProfileData.id}" hidden>
            <label for="userProfile" style="width:100%; text-align:center">
                <img src='${profImage[profImage.length - 1]}' id="image" style="height: 220px; width: 240px; border-radius: 50%;"/>
            </label>
            <input type="file" id="userProfile" name="ProfileImage" onchange="changeProfileImage()" hidden/>
            <br /><br />
            <label for="UserName" class="text-primary" style="text-align:left">UserName</label>
            <input type="text" name="UserName" value="${EditProfileData.userName}" class="form-control" id="UserName" disabled>
            <br />
            <label for="PhoneNo" class="text-primary" style="text-align:left">Contact Number</label>
            <input type="text" name="PhoneNo" value="${EditProfileData.phoneNumber}" class="form-control" id="PhoneNo" maxlength="10" minlength="10">
            <br />
            <label for="Email" class="text-primary" style="text-align:left">Email</label>
            <input type="text" name="Email" value="${EditProfileData.email}" class="form-control" id="Email">
            <br /><br />
            <div style="width:100%; text-align:center">
                <input type="submit" class="btn btn-primary" value="CHANGE">
            </div>
        </form>`,
        showConfirmButton: false
    });
}

function removeUser(id, groupId, userName) {
    var answer = confirm(`You Sure you Want To Remove '${userName}' ?`);
    if (answer) {
        window.location.href = `/Services/RemoveFromGroup?userId=${id}&groupId=${groupId}`;
    }
}

async function editMembers() {
    if (isGroup) {
        const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
        let response = await fetch(`Services/GetGroupMembers?groupId=${groupId}`);
        let data = await response.json();
        let userList = [];
        let htmlString = `<table style="margin-top:20px">`;
        data.forEach(function (ele) {
            if (Number(sender) != ele.id) {
                userList.push(ele.id);
                htmlString += `<tr>
                    <td style="padding-left:100px;padding-right:100px; padding-bottom:25px">
                        ${ele.userName}
                    </td>
                    <td style="padding-bottom:25px;">
                        <input type="button" value="REMOVE" class="btn btn-outline-danger" onclick="removeUser(${ele.id}, '${groupId}', '${ele.userName}')"/>
                    </td>
                </tr>`;
            }
        });
        htmlString += `</table>`;
        Swal.fire({
            title: 'Edit Group',
            html: htmlString,
            confirmButtonText: 'Add Member'
        }).then(async (result) => {
            if (result.isConfirmed) {
                let allUserResponse = await fetch(`/Services/getAllUsers?id=${sender}`);
                let allUserData = await allUserResponse.json();
                let selectString = `<select name="UserId" class="form-control" id="users" style="height:120px" multiple>`;
                allUserData.forEach(function (ele) {
                    if (userList.includes(ele.id)) {
                        selectString += `<option value="${ele.id}" disabled>${ele.userName}</option>`
                    }
                    else {
                        selectString += `<option value="${ele.id}">${ele.userName}</option>`
                    }
                });
                selectString += `</select>`;
                Swal.fire({
                    title: "Add Members",
                    text: "Use ctrl then Select To Add Multiple Members",
                    html: `<form action="/Services/AddToGroup" method="post">
                        <div style="text-align:left">
                            <input type="text" name="GroupId" value="${groupId}" hidden />
                            <label  for="users" class="text-primary">Select Users</label>
                            ${selectString}
                        </div>
                        <br /><br />
                        <input type="submit" value="ADD" class="btn btn-primary">
                    </form>`,
                    showConfirmButton: false
                });
            }
        });
    }
}

async function getValue() {
    if (!isGroup && receiver == 0) {
        alert('Please Select Group Or User To Send File!');
        return;
    }

    //var formData = new FormData(document.getElementById('fileUploadForm'));
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    let formData = new FormData();
    formData.append('SenderId', Number(sender));
    formData.append('ReceiverId', isGroup ? null : receiver)
    formData.append('GroupId', isGroup ? groupId : null);

    const fileEle = document.getElementById('file');
    for (const file of fileEle.files) {
        formData.append('Files', file);
    }
    let response = await fetch('/Services/SendFile', {
        method: 'POST',
        body: formData,
    });
    let fileData = await response.json();
    fileData.upload.files.forEach((ele) => {
        fileData.urls.type.push(ele.contentType)
    });
    if (globalDay != -1) {
        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                </div>
            </div>`;
        globalDay = -1;
    }
    /*let fileCounter = 0;*/
    let htmlStr = ``;
    fileData.urls.urls.forEach((ele, index) => {
        htmlStr += `<div class="row ps-5 pe-5 pt-2 pb-2">
            <div class="col-2"></div>
            <div class="col-10 text-end">`;
        if (fileData.upload.files[index].contentType.slice(0, 5) == "image") {
            htmlStr += `<a href="${ele}" target='_blank' download><img src="${ele}" height="300px" id="${ele}" /></a>`;
            if (isGroup) {
                document.getElementById(`${groupId}:LastMessage`).innerText = '🎞 File';
            }
            else {
                document.getElementById(`${receiver}:lastMessage`).innerText = '🎞 File';
            }
        }
        else {
            htmlStr += `<embed src="${ele}" height="300px" id="${ele}" />`;
            if (isGroup) {
                document.getElementById(`${groupId}:LastMessage`).innerText = '🎞 File';
            }
            else {
                document.getElementById(`${receiver}:lastMessage`).innerText = '🎞 File';
            }
        }
        htmlStr += `<br />
                <span style="padding:5px; font-size:13px; opacity: 0.7;">
                    ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                </span>
            </div>
        </div>`;
        /*fileCounter++;*/
    });
    messages.innerHTML += htmlStr;
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
    if (isGroup) {
        await connection.invoke("SendFileToGroup", fileData.upload, fileData.urls);
    }
    else if (receiver != 0) {
        await connection.invoke("SendFile", fileData.upload, fileData.urls);
    }
    
}

function removeSuggetion() {
    document.getElementById('searchSuggetion').style.visibility = 'hidden';
}

function showSuggetion() {
    document.getElementById('searchSuggetion').style.visibility = 'visible';
}

async function search() {
    const sender = ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0];
    let suggetionEle = document.getElementById('searchSuggetion');
    let searchText = document.getElementById('searchResult').value.trim();
    if (searchText != '') {
        let searchResponse = await fetch(`/Services/getSearchResult?name=${searchText}`);
        const searchResult = await searchResponse.json();
        console.log(searchResult);
        suggetionEle.innerHTML = '';
        searchResult.forEach((ele) => {
            if (ele.userId != Number(sender)) {
                suggetionEle.innerHTML += `<div class="row p-2" style="align-items:center; width:100%; margin:0 2px;" onclick="getSearchResult(${ele.userId})">
					<div class="col-3 text-center"><img src="${ele.profileImage.split('wwwroot')[1]}" style="border-radius:50%;" height="50px" width="55px" /></div>
					<div class="col-9" style="font-size: 25px; margin:-2px 0 0 -15px;">${ele.name}</div>
				</div>`;
            }
        });
    }
    else {
        document.getElementById('searchSuggetion').innerHTML = '';
    }
}

async function getSearchResult(id) {
    document.getElementById('searchSuggetion').innerHTML = '';
    document.getElementById('searchResult').value = '';
    displayChat(id);
}