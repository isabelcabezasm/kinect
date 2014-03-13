window.onload = function () {
    var status = document.getElementById("status");
    var canvas = document.getElementById("canvas");
    var context = canvas.getContext("2d");

    if (!window.WebSocket) {
        status.innerHTML = "Your browser does not support web sockets!";
        return;
    }

    status.innerHTML = "Connecting to server...";

    // Initialize a new web socket.
    var socket = new WebSocket("ws://localhost:8181/KinectHtml5");

    // Connection established.
    socket.onopen = function () {
        status.innerHTML = "Connection successful.";
    };

    // Connection closed.
    socket.onclose = function () {
        status.innerHTML = "Connection closed.";
    }

    // Receive data FROM the server!
    socket.onmessage = function (event) {
        if (typeof event.data === "string") {
            status.innerHTML = "Kinect skeleton received.";

            // 1. Get the data in JSON format.
            var jsonObject = JSON.parse(event.data);
            document.getElementById("textstatus").innerHTML = jsonObject["1"].value;
            var array = jsonObject["1"].value.split(',');
            

            context.clearRect(0, 0, canvas.width, canvas.height);



                context.fillStyle = "#FF0000";
                context.beginPath();
                context.arc(array[0], array[1], 10, 0, Math.PI * 2, true);
                context.closePath();
                context.fill();




            //// 2. Display the skeleton joints.
            //for (var i = 0; i < jsonObject.skeletons.length; i++) {
            //    for (var j = 0; j < jsonObject.skeletons[i].joints.length; j++) {
            //        var joint = jsonObject.skeletons[i].joints[j];

            //        // Draw!!!
            //        context.fillStyle = "#FF0000";
            //        context.beginPath();
            //        context.arc(joint.x, joint.y, 10, 0, Math.PI * 2, true);
            //        context.closePath();
            //        context.fill();
            //    }
            //}


            // 2. Display the  joint.

            //for (var j = 1; j <= jsonObject.length; j++) {
            //    context.fillStyle = "#FF0000";
            //    context.beginPath();
            //    context.arc(jsonObject[j].Key, jsonObject[j].Value, 10, 0, Math.PI * 2, true);
            //    context.closePath();
            //    context.fill();
            //}
            




            // Inform the server about the update.
            socket.send("Skeleton updated on: " + (new Date()).toDateString() + ", " + (new Date()).toTimeString());
        }
    };
};