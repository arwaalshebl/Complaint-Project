const connection = new signalR.HubConnectionBuilder()
    .withUrl("/complaintHub")
    .build();

connection.on("ReceiveComplaintToast", function (actionType, messageText) {
    console.log("Real-time action caught: " + actionType);

    // const toastElement = document.getElementById('ReceiveComplaintToast');
    // const messageElement = document.getElementById('toastMessage');
    // const timeElement = document.getElementById('toastTime');

    const toastElement = document.getElementById('ReceiveComplaintToast');
    const headerElement = toastElement.querySelector('.toast-header');
    const titleElement = toastElement.querySelector('.toast-header strong');
    const iconElement = toastElement.querySelector('.toast-header i');
    const bodyTitleElement = toastElement.querySelector('.toast-body h5');
    const messageElement = document.getElementById('toastMessage');
    const timeElement = document.getElementById('toastTime');

    const currentTime = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    if (timeElement) timeElement.innerText = currentTime;

    if (actionType === "NewComplaint") {
       
        headerElement.className = "toast-header bg-warning text-white rounded-top-4 border-0 py-2";
        iconElement.className = "bi bi-star-fill me-2";
        titleElement.innerText = "New Complaint";
        bodyTitleElement.innerText = "New Complaint!";
        bodyTitleElement.className = "fw-bold text-primary";
    }
    else if (actionType === "ComplaintReopened") {
        headerElement.className = "toast-header bg-danger text-white rounded-top-4 border-0 py-2";
        iconElement.className = "bi bi-exclamation-triangle-fill me-2";
        titleElement.innerText = "Complaint Reopened";
        // bodyTitleElement.innerText = "Urgent Action Required!";
        bodyTitleElement.className = "fw-bold text-danger";
    }
    else if (actionType === "ComplaintAssigned") {
        headerElement.className = "toast-header bg-primary text-white rounded-top-4 border-0 py-2";
        iconElement.className = "bi bi-check-triangle-fill me-2";
        titleElement.innerText = "Complaint Assigned";
        // bodyTitleElement.innerText = "Urgent Action Required!";
        bodyTitleElement.className = "fw-bold text-primary";
    }
    else if (actionType === "ComplaintReplied") {
        headerElement.className = "toast-header bg-info text-white rounded-top-4 border-0 py-2";
        iconElement.className = "bi bi-check-triangle-fill me-2";
        titleElement.innerText = "Complaint Replied";
        // bodyTitleElement.innerText = "Urgent Action Required!";
        bodyTitleElement.className = "fw-bold text-info";
    }
    else if (actionType === "ComplaintApprove") {
        headerElement.className = "toast-header bg-info text-white rounded-top-4 border-0 py-2";
        iconElement.className = "bi bi-check-triangle-fill me-2";
        titleElement.innerText = "Complaint Approve";
        // bodyTitleElement.innerText = "Urgent Action Required!";
        bodyTitleElement.className = "fw-bold text-info";
    }

    if (messageElement) {
        messageElement.innerText = messageText;
    }

    const bsToast = new bootstrap.Toast(toastElement);
    bsToast.show();

    if (window.location.pathname.includes("/Complaints") || window.location.pathname.includes("/Complaints/Index")) {
        setTimeout(function () {
            location.reload();
        }, 4000);
    }
});




connection.start().then(function () {
    console.log("SignalR Connected!");

    connection.invoke("JoinServicesGroup").then(function () {
        console.log("Successfully joined PatientServicesGroup via JS Invoke!");
    }).catch(function (err) {
        return console.error("Group join failed: " + err.toString());
    });

}).catch(function (err) {
    return console.error(err.toString());
});
