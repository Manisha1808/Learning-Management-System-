$(document).ready(function () {

    debugger;

    function showMessage(message) {
        $("#messageText").text(message);
        $("#messageDialog").dialog("open");
    }

    $("#messageDialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            OK: function () {
                $(this).dialog("close");
            }
        }
    });

   
    $("#userGrid").jqGrid({
        url: "/Admin/Admin/GetUsers",
        datatype: "json",
        colNames: [
            "Name",
            "Email",
            "Role",
            "Course",
            "Enrollment Date",
            "UserStatus",
            "Created Date",
            "Created By",
            "Actions"
        ],
        colModel: [
            {
                name: "Name",
                width: 100
            },
            {
                name: "Email",
                width: 100
            },
            {
                name: "Role",
                width: 100
            },
            {
                name: "Course",
                width: 100,
            },
            {
                name: "EnrollmentDate",
                width: 150,
                formatter: function (cellvalue) {
                    if (!cellvalue) {
                        return "N/A";
                    }
                    var date = new Date(parseInt(cellvalue.substr(6)));
                    return ("0" + date.getDate()).slice(-2) + "-" +
                        ("0" + (date.getMonth() + 1)).slice(-2) + "-" +
                        date.getFullYear();
                }
            },
            {
                name: "UserStatus",
                width: 100
            },
            {
                name: "CreatedDate",
                formatter: function (cellvalue) {
                    var date = new Date(parseInt(cellvalue.substr(6)));
                    return ("0" + date.getDate()).slice(-2) + "-" +
                        ("0" + (date.getMonth() + 1)).slice(-2) + "-" +
                        date.getFullYear();
                }
            },
            {
                name: "CreatedBy",
                width: 100
            },
            {
                name: "Actions",
                width: 180,
                sortable: false,

                formatter: function (cellvalue, options, rowObject) {

                    var statusButton = "";

                    if (rowObject.UserStatus === "Active") {

                        statusButton =
                            '<button type="button" ' +
                            'class="btn btn-warning btn-sm btn-status" ' +
                            'data-id="' + rowObject.UserId + '" ' +
                            'data-status="false">' +
                            'Deactivate</button>';

                    }
                    else {

                        statusButton =
                            '<button type="button" ' +
                            'class="btn btn-success btn-sm btn-status" ' +
                            'data-id="' + rowObject.UserId + '" ' +
                            'data-status="true">' +
                            'Activate</button>';
                    }
                    return '<button type="button" ' +
                        'class="btn btn-primary btn-sm btn-edit" ' +
                        'data-id="' + rowObject.UserId + '">' +
                        'Edit</button> ' +
                        statusButton;
                }
            }
        ]


    });
    
    // SEARCH
    $("#btnSearchUser").click(function () {
        var searchText = $("#txtSearchUser").val();
        var roleId = $("#ddlSearchRole").val();
        if (roleId === "") {
            roleId = null;
        }
        $("#userGrid")
            .jqGrid("setGridParam", {
                url: "/Admin/Admin/SearchUsers",
                datatype: "json",
                postData: {
                    SearchText: searchText,
                    RoleId: roleId
                }
            })
            .trigger("reloadGrid");
    });

    // CLEAR
    $("#btnClearSearch").click(function () {
        $("#txtSearchUser").val("");
        $("#ddlSearchRole").val("");

        $("#userGrid")
            .jqGrid("setGridParam", {
                url: "/Admin/Admin/GetUsers",
                datatype: "json",
                postData: {}
            }).trigger("reloadGrid");
    });
    
    // EDIT
    $(document).on("click", ".btn-edit", function () {
        var userId = $(this).data("id");
        $.ajax({
            url: "/Admin/Admin/GetUserById",
            type: "GET",
            data: { id: userId },
            success: function (response) {
                if (response != null) {
                    $("#txtUserId").val(response.UserId);
                    $("#txtFirstName").val(response.FirstName);
                    $("#txtLastName").val(response.LastName);
                    $("#txtEmail").val(response.Email);
                    $("#txtRole").val(response.RoleId);                    
                    $("#txtStartDate").val(response.StartDate);
                    $("#txtEndDate").val(response.EndDate);
                    $("#chkCourseActive").prop("checked", response.CourseIsActive);
                    $("#userModal").modal("show");
                }
            },
            error: function () {
                showMessage("Error while loading user details.");
            }
        });
    });

    // UPDATE
    $("#btnUpdateUser").click(function () {

        var userId = $("#txtUserId").val();
        var firstName = $("#txtFirstName").val();
        var lastName = $("#txtLastName").val();
        var email = $("#txtEmail").val();
        var roleId = $("#txtRole").val();
        var startdate = $("#txtStartDate").val();
        var enddate = $("#txtEndDate").val();
        var courseIsActive = $("#chkCourseActive").is(":checked");

        $.ajax({
            url: "/Admin/Admin/UpdateUser",
            type: "POST",

            data: {
                UserId: userId,
                FirstName: firstName,
                LastName: lastName,
                Email: email,
                RoleId: roleId,
                StartDate: startdate,
                EndDate: enddate,
                CourseIsActive: courseIsActive
            },

            success: function (response) {
                if (response.success) {
                    showMessage(response.message);
                    $("#userModal").modal("hide");
                    $("#userGrid").jqGrid("setGridParam", { datatype: "json" }).trigger("reloadGrid"); }
                else {showMessage("Update failed.");}
            },

            error: function () {
                showMessage("Error while updating user.");
            }
        });
    });
    // CLOSE
    $("#btnCloseUserModal").click(function () {
        $("#userModal").modal("hide");
    });

    // ACTIVATE / DEACTIVATE
     $(document).on("click", ".btn-status", function () {
        var userId = $(this).data("id");
        var isActive = $(this).data("status") === true ||
            $(this).data("status") === "true";
        $.ajax({
            url: "/Admin/Admin/UpdateUserStatus",
            type: "POST",
            data: {
                UserId: userId,
                IsActive: isActive
            },
            success: function (response) {
                if (response.success) {
                    showMessage(response.message);
                    $("#userGrid").jqGrid("setGridParam", { datatype: "json" }).trigger("reloadGrid");
                }
                else {showMessage("Unable to update user status.");}
            },

            error: function () {showMessage("Error while updating user status.");}
        });
    });

});

