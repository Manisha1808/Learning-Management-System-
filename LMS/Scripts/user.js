$(document).ready(function () {
    debugger
    alert("user.js loaded");

    //alert("jqGrid type: " + typeof $.fn.jqGrid);

    $("#userGrid").jqGrid({
        
        datatype: "local",

        colNames: [
            "ID",
            "Name",
            "Email",
            "Role",
            "Course",
            "Enrollment Date",
            "Status",
            "Actions"
        ],

        colModel: [

            {
                name: "UserId",
                key: true,
                width: 60
            },

            {
                name: "Name",
                width: 150
            },

            {
                name: "Email",
                width: 200
            },

            {
                name: "Role",
                width: 100
            },

            {
                name: "Course",
                width: 120
            },

            {
                name: "EnrollmentDate",
                width: 130
            },

            {
                name: "Status",
                width: 120
            },

            {
                name: "Actions",
                width: 150
            }
        ],

 });

});
