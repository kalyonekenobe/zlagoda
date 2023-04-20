let printEmployeesButton = document.getElementById("print-employees");
if (printEmployeesButton) {
    printEmployeesButton.onclick = event => {
        let employeesTable = document.getElementById("employees-table");
        if (employeesTable) {
            window.print();
        }
    }
}