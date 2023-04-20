let printEmployeesButton = document.getElementById("print-employees");
if (printEmployeesButton) {
    printEmployeesButton.onclick = event => {
        let employeesTable = document.getElementById("employees-table");
        if (employeesTable) {
            window.print();
        }
    }
}

let closeErrorButtons = document.querySelectorAll(".error .close");
closeErrorButtons.forEach(button => button.onclick = event => {
    let parent = button.parentElement;
    parent.style.display = "none";
});