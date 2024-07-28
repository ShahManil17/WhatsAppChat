const input = document.getElementById('profile');
input.addEventListener('change', event => {
    let fReader = new FileReader();
    fReader.readAsDataURL(input.files[0]);
    fReader.onload = function (event) {
        let img = document.getElementById('profileImage');
        img.src = event.target.result;
    }
})