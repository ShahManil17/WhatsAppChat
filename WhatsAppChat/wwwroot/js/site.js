let receiver = 0;
let groupId = ``;
let isGroup = false;
let globalDay = 0;

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
        document.getElementById(`${rec}:lastMessage`).innerText = messageText;
    }
    else {
        const numberEle = document.getElementById(`${rec}`);
        if (numberEle.innerText == '') {
            numberEle.style.padding = '5px';
            numberEle.innerText = '1';

            // Display Last Message
            document.getElementById(`${rec}:lastMessage`).innerText = messageText;
        }
        else {
            numberEle.innerText = `${Number(numberEle.innerText) + 1}`;
            document.getElementById(`${rec}:lastMessage`).innerText = messageText;
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
            numberEle.style.padding = '5px';
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

connection.on("ReceiveFile", (sender, rec, urls) => {
    console.log(`Sender : ${sender}`)
});

connection.on("ReceiveFromGroup", async (senderId, senderName, group, message) => {
    console.log(`Sender : ${senderName}`)
});

//connection.on('ReceiveFile', async (sender, rec, messageUrl) => {
//    if (rec == receiver && isGroup == false) {
//        let response = await fetch(`${window.location.origin}/Services/markAsRead?senderId=${receiver}&receiverId=${Number(sender)}`);
//        if (globalDay != -1) {
//            messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
//                <div class="col text-center">
//                    <span style="border-radius: 10px; padding: 7px; background-color: lavender;">Today</span>
//                </div>
//            </div>`;
//            globalDay = -1;
//        }
//        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2" style="align-items:end">
//            <div class="col-10" style="overflow-wrap: anywhere;">
//            <span style="background-color: rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">${messageText}</span>
//            <br/>
//            <span style="padding:5px; font-size:13px; opacity: 0.7;">
//                ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
//            </span>
//            </div>
//            <div class="col-2"></div>
//        </div>`;
//        document.getElementById(`${rec}:lastMessage`).innerText = messageText;
//    }
//    else {
//        const numberEle = document.getElementById(`${rec}`);
//        if (numberEle.innerText == '') {
//            numberEle.style.padding = '5px';
//            numberEle.innerText = '1';

//            // Display Last Message
//            document.getElementById(`${rec}:lastMessage`).innerText = messageText;
//        }
//        else {
//            numberEle.innerText = `${Number(numberEle.innerText) + 1}`;
//            document.getElementById(`${rec}:lastMessage`).innerText = messageText;
//        }
//    }
//    messages.scrollTop = messages.scrollHeight;
//    messages.scrollIntoView();
//});

 function send() {
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
    messages.scrollTop = messages.scrollHeight;
    messages.scrollIntoView();
}

async function displayChat(id) {
    let temp = new Date().toLocaleDateString();
    const currentDate = new Date(temp);
    isGroup = false;
    const numberEle = document.getElementById(`${id}`);
    numberEle.style.padding = '0';
    numberEle.innerText = '';
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
        data.message.forEach(function (ele, index) {
            var messageDate = new Date(Date.parse(ele.sendTime));
            let dateDiff = Math.floor((currentDate.getTime() - messageDate.getTime()) / 86400000);
            if (!dateArr.includes(dateDiff)) {
                dateArr.push(dateDiff);
                if (dateDiff == -1) {
                    messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                        <div class="col text-center">
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
            if (ele.senderId.toString() == ('; ' + document.cookie).split(`; userId=`).pop().split(';')[0]) {
                messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-2"></div>
                    <div class="col-10 text-end" style="overflow-wrap: anywhere;">
                    <span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${ele.message}</span>
                    <br/>
                    <span style="padding:5px; font-size:13px; opacity: 0.7;">
                        ${ele.sendTime.toString().slice(11, 16)}
                    </span>
                    </div>
                </div>`;
            }
            else {
                messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-10 text-start" style="overflow-wrap: anywhere;">
                        <span style="background-color:rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">${ele.message}</span>
                        <br/>
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)}
                        </span>
                    </div>
                    <div class="col-2"></div>
                </div>`;
            }
        });
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
            if (ele.senderId.toString() == sender) {
                messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-2"></div>
                    <div class="col-10 text-end" style="overflow-wrap: anywhere;">
                        <span style="background-color:rgb(220, 248, 198, 0.6); padding:5px; border-radius:10px">${ele.message}</span>
                        <br/>
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)}
                        </span>
                    </div>
                </div>`;
            }
            else {
                messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                    <div class="col-10 text-start" style="overflow-wrap: anywhere;">
                        <span style="background-color:rgb(240, 255, 255, 0.6); padding:5px; border-radius:10px">
                            ${ele.message}
                        </span>
                        <br />
                        <span style="padding:5px; font-size:13px; opacity: 0.7;">
                            ${ele.sendTime.toString().slice(11, 16)} : ${ele.userName}
                        </span>
                    </div>
                    <div class="col-2"></div>
                </div>`;
            }
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
    let EditProfileRes = await fetch(`/Services/getSingleUser`);
    let EditProfileData = await EditProfileRes.json();
    profImage = EditProfileData.profileImage.split('wwwroot/');
    Swal.fire({
        title: 'Edit Profile',
        html: `<form action="/Services/editProfile" method="post" enctype="multipart/form-data" style="text-align:left">
            <input type="number" name="Id" value="${EditProfileData.id}" hidden>
            <label for="userProfile" style="width:100%; text-align:center">
                <img src='${profImage[profImage.length - 1]}' id="image" style="height:150px; border-radius:100%"/>
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
    console.log(fileData);

    if (globalDay != -1) {
        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
                <div class="col text-center">
                    <span style="border-radius: 10px; padding: 5px; background-color: lavender;">Today</span>
                </div>
            </div>`;
        globalDay = -1;
    }
    let fileCounter = 0;
    for (const file of fileEle.files) {
        messages.innerHTML += `<div class="row ps-5 pe-5 pt-2 pb-2">
            <div class="col-2"></div>
            <div class="col-10 text-end">
                <embed src="" height="300px" id="file${fileCounter}">
                <br />
                <span style="padding:5px; font-size:13px; opacity: 0.7;">
                    ${currentTime.toLocaleTimeString('en-US', { hour12: false }).slice(0, 5)}
                </span>
            </div>
        </div>`;
        fileCounter++;
    }
    document.getElementById(`${receiver}:lastMessage`).innerText = '🎞 File';
    for (let i = 0; i < fileCounter; i++) {
        let fReader = new FileReader();
        fReader.readAsDataURL(fileEle.files[i]);
        fReader.onload = function (event) {
            let img = document.getElementById(`file${i}`);
            img.src = event.target.result;
        }
    }
    if (isGroup) {
        //for (let i = 0; i < fileCounter; i++) {
        //    document.getElementById(`file${i}`).src = fileData.urls[i];
        //}
        await connection.invoke("SendFileToGroup", fileData.upload, fileData.urls);
    }
    else if (receiver != 0) {
        //for (let i = 0; i < fileCounter; i++) {
        //    document.getElementById(`file${i}`).src = fileData.urls[i];
        //}
        await connection.invoke("SendFile", fileData.upload, fileData.urls);
    }
}