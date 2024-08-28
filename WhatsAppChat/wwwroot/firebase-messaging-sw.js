// Get registration token. Initially this makes a network call, once retrieved
// subsequent calls to getToken will return from cache.
// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here. Other Firebase libraries
// are not available in the service worker.

importScripts('https://www.gstatic.com/firebasejs/9.23.0/firebase-app-compat.js', 'https://www.gstatic.com/firebasejs/9.0/firebase-messaging-compat.js');

// importScripts('https://www.gstatic.com/firebasejs/9.0/firebase-messaging.js');

// Initialize the Firebase app in the service worker by passing in
// your app's Firebase config object.
// https://firebase.google.com/docs/web/setup#config-object

firebase.initializeApp({
    apiKey: "AIzaSyClvLAKa1kFHqjKW6N82lx9yIFiqk2731s",
    authDomain: "whatsappchat-843ef.firebaseapp.com",
    projectId: "whatsappchat-843ef",
    storageBucket: "whatsappchat-843ef.appspot.com",
    messagingSenderId: "384308968125",
    appId: "1:384308968125:web:db31718fa907bd7e5095d4",
    measurementId: "G-16172BGRPN"
});

const messaging = firebase.messaging();

messaging.onMessage((payload) => {
    console.log('Message received. ', payload);
});


messaging.getToken({
    vapidKey: 'BE_otnE3lx4jeXkfXTUm2U-mbx_gKGajrElkSkWtM0Myfm36iC7Tv_KT2l97YiwAWfNJjMqT1QWmrKpLa-S4MB4'
}).then((currentToken) => {
    console.log(currentToken)
    if (currentToken) {
        // Send the token to your server and update the UI if necessary
        // ...
    } else {
        // Show permission request UI
        console.log('No registration token available. Request permission to generate one.');
        // ...
    }

}).catch((err) => {
    console.log('An error occurred while retrieving token. ', err);
    // ...
});