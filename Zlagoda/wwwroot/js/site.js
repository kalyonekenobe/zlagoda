let models = ['employees', 'categories', 'clients', 'products', 'store-products', 'checks'];

models.forEach(modelName => {
    let printButton = document.getElementById(`print-${modelName}`);
    if (printButton) {
        printButton.onclick = event => {
            let table = document.getElementById(`${modelName}-table`);
            if (table) {
                window.print();
            }
        }
    }

    let findModelDetailsInput = document.getElementById(`get-${modelName}-info-input`);
    let button = document.getElementById(`get-${modelName}-info-btn`);
    if (findModelDetailsInput && button) {
        findModelDetailsInput.oninput = event => {
            let value = event.target.value;
            button.setAttribute('href', `brief-info/${value}`);
        }
    }
});

let addCloseButtonsClickEvents = () => {
    let closeErrorButtons = document.querySelectorAll(".error .close");
    closeErrorButtons.forEach(button => button.onclick = event => {
        let parent = button.parentElement;
        parent.style.display = "none";
    });
}

addCloseButtonsClickEvents();

let storeProductParentSelect = document.getElementById('ParentUPC');
let storeProductQuantityInput = document.querySelectorAll('#StoreProduct_products_number')[1];
let updateStoreParentSelect = async () => {
    if (storeProductParentSelect) {
        if (storeProductParentSelect.offsetParent === null) {
            if (storeProductQuantityInput) {
                storeProductQuantityInput.removeAttribute('max');
            }
            return;
        } 
        let value = storeProductParentSelect.value;
        let url = `${location.origin}/api/store-products/${value}`;
        await fetch(url).then(async response => await response.json()).then(response => {
            if (storeProductQuantityInput) {
                let max = response.products_number;
                storeProductQuantityInput.setAttribute('max', max);
            }
        });
    }
}
if (storeProductParentSelect) {
    if (storeProductParentSelect.offsetParent !== null) {
        updateStoreParentSelect();
    } 
    storeProductParentSelect.onchange = async event => {
        await updateStoreParentSelect();
    }
}

let storeProductTypeSelector = document.querySelector('.store-product-type-selector');
if (storeProductTypeSelector) {
    let buttons = storeProductTypeSelector.querySelectorAll('span');
    let containers = document.querySelectorAll('.store-product-type-container');
    buttons.forEach(button => button.onclick = event => {
        buttons.forEach(button => button.classList.remove('selected'));
        button.classList.add('selected');
        containers.forEach(container => {
            if (container.dataset.type === button.dataset.type) {
                container.classList.add('visible');
            } else {
                container.classList.remove('visible');
            }
        })
        updateStoreParentSelect();
    });
}

let storeProductSelect = document.getElementById('store-product-select');
let checkStoreProductQuantityInput = document.getElementById('store-product-quantity');

if (storeProductSelect) {
    let updateQuantityInputAttributes = async () => {
        let quantityInput = document.getElementById('store-product-quantity');
        let url = location.href.replace('checks/create', 'api/store-products');
        await fetch(url).then(async response => await response.json()).then(result => {
            let selectedProduct = storeProductSelect.value;
            let maxQuantity = result.find(product => product.UPC === selectedProduct).products_number;
            quantityInput.setAttribute('max', maxQuantity);
        });
    }
    updateQuantityInputAttributes().then(async () => {
        storeProductSelect.onchange = async event => {
            await updateQuantityInputAttributes();
            if (checkStoreProductQuantityInput) {
                checkStoreProductQuantityInput.value = 0;
            }
        }
    });
}

if (checkStoreProductQuantityInput) {
    let quantityChangeEvent = event => {
        let max = checkStoreProductQuantityInput.getAttribute('max');
        checkStoreProductQuantityInput.value = Math.min(event.target.value, max);
    }
    checkStoreProductQuantityInput.oninput = event => quantityChangeEvent(event);
}

let createCheckButton = document.getElementById('create-check-button');
let checkProducts = [];

let addStoreProductToCheckButton = document.getElementById('add-product-to-check-btn');
if (addStoreProductToCheckButton) {
    let totalCheckSum = document.getElementById('total-check-sum');
    addStoreProductToCheckButton.onclick = async event => {
        let productInput = document.getElementById('store-product-select');
        let quantityInput = document.getElementById('store-product-quantity');
        let checkProductsTable = document.getElementById('check-products-table');
        if (productInput.value && quantityInput.value && quantityInput.value > 0) {
            let url = location.href.replace('checks/create', 'api/store-products');
            await fetch(url).then(async response => await response.json()).then(result => {
                let checkProduct = result.find(product => product.UPC === productInput.value);
                if (checkProduct) {
                    let row = checkProductsTable.querySelector(`tr[data-product-upc='${checkProduct.UPC}']`);
                    if (!row) {
                        if (Number.parseInt(quantityInput.value) <= checkProduct.products_number || checkProduct.products_number !== 0) {
                            if (Number.parseInt(quantityInput.value) > checkProduct.products_number) {
                                alert("Перевищення максимально доступної кількості товару! Кількість була округлена до максимального значення.");
                            }
                            checkProduct.product_number = Math.max(1, Math.min(Number.parseInt(quantityInput.value), Number.parseInt(quantityInput.getAttribute('max'))));
                            checkProducts.push(checkProduct);
                            row = checkProductsTable.insertRow();
                            row.setAttribute('data-row-id', checkProductsTable.rows.length - 1);
                            row.setAttribute('data-product-upc', checkProduct.UPC);
                        } else {
                            alert("Наразі даний товар було розпродано!");
                        }
                    } else {
                        checkProduct = checkProducts.find(product => product.UPC === checkProduct.UPC);
                        if (checkProduct.product_number + Number.parseInt(quantityInput.value) > checkProduct.products_number) {
                            alert("Перевищення максимально доступної кількості товару! Кількість була округлена до максимального значення.");
                        }
                        checkProduct.product_number = Math.min(checkProduct.product_number + Number.parseInt(quantityInput.value), checkProduct.products_number);
                    }
                    if (checkProduct && row) {
                        row.innerHTML = `
                            <td>${checkProduct.product.product_name} ${checkProduct.promotional_product ? "(акційний)" : ""}</td>
                            <td>${checkProduct.product.category.category_name}</td>
                            <td>
                                ${new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(checkProduct.selling_price).replace(',', ' ')}$
                            </td>
                            <td>${checkProduct.product_number} шт.</td>
                            <td>
                                ${new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(checkProduct.product_number * checkProduct.selling_price).replace(',', ' ')}$
                            </td>
                            <td width="112">
                                <a class="btn btn-sm btn-outline-danger delete-btn">
                                    <i class="fa-solid fa-trash me-2"></i>
                                    Видалити
                                </a>
                            </td>
                        `;
                        let deleteButton = row.querySelector('.delete-btn');
                        if (deleteButton) {
                            deleteButton.onclick = event => {
                                checkProducts = checkProducts.filter(product => product.UPC !== row.dataset.productUpc);
                                totalSum = checkProducts.reduce((partialSum, item) => partialSum + (item.product_number * item.selling_price), 0);
                                checkProductsTable.deleteRow(row.dataset.rowId);
                                totalCheckSum.innerHTML = `${new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(totalSum).replace(',', ' ')}$`;
                                if (checkProducts.length === 0) {
                                    createCheckButton.setAttribute('disabled', true);
                                }
                            }
                        }
                    }
                    let totalSum = checkProducts.reduce((partialSum, item) => partialSum + (item.product_number * item.selling_price), 0);
                    if (checkProducts.length > 0) {
                        createCheckButton.removeAttribute('disabled');
                    }
                    quantityInput.value = 1;
                    totalCheckSum.innerHTML = `${new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(totalSum).replace(',', ' ') }$`;
                }
            });
        }
    }
}

let createCheckForm = document.getElementById('create-check-form');
if (createCheckForm) {
    createCheckForm.onsubmit = async event => {
        event.preventDefault();
        let checkNumber = document.getElementById('Check_check_number').value;
        let cardNumber = document.getElementById('Check_card_number').value;
        let idEmployee = document.getElementById('Check_id_employee').value;
        if (checkNumber && idEmployee) {
            let check = {
                "check_number": checkNumber,
                "id_employee": idEmployee,
                "card_number": cardNumber,
                "print_date": new Date().toISOString().split('T')[0],
                "total_sum": 0,
                "vat": 0,
                "products": checkProducts
            };
            fetch(location.href, {
                method: "POST",
                body: JSON.stringify(check),
                headers: {
                    'Accept': 'application/json; charset=utf-8',
                    'Content-Type': 'application/json;charset=UTF-8'
                },
            }).then(async response => {
                if (response.ok) {
                    checkProducts = [];
                    let url = location.href.replace('checks/create', 'checks/list');
                    location.replace(url);
                } else {
                    let errors = await response.json();
                    errors.forEach(error => {
                        let errorElement = document.createElement('span');
                        errorElement.classList.add('error');
                        errorElement.innerHTML = error;
                        let closeErrorElement = document.createElement('i');
                        closeErrorElement.classList.add('fa-solid');
                        closeErrorElement.classList.add('fa-xmark');
                        closeErrorElement.classList.add('close');
                        errorElement.appendChild(closeErrorElement);
                        createCheckForm.insertBefore(errorElement, createCheckForm.firstChild);
                        addCloseButtonsClickEvents();
                    })
                }
            }).catch(error => {
                console.log(error);
            });
        }
    }
}

let showCheckDateRangeCheckbox = document.getElementById('check-creation-date-range');
if (showCheckDateRangeCheckbox) {
    let endDate = document.getElementById('search-check-end-date');
    let labelDateRange = document.getElementById('label-date-range');
    let updateCheckboxState = () => {
        let dateToContainer = document.querySelector('.dropdown .date-to-container');
        if (showCheckDateRangeCheckbox.checked) {
            dateToContainer.classList.add('d-none');
            labelDateRange.innerHTML = "Дата створення чеків (конкретна дата)";
            endDate.value = "";
        } else {
            dateToContainer.classList.remove('d-none');
            labelDateRange.innerHTML = "Дата створення чеків (проміжок часу)";
        }
    }
    updateCheckboxState();
    showCheckDateRangeCheckbox.onchange = event => {
        updateCheckboxState();
    }   
}

let searchCheckButton = document.getElementById('search-check-button');
if (searchCheckButton) {
    searchCheckButton.onclick = event => {
        let startDate = document.getElementById('search-check-start-date');
        let endDate = document.getElementById('search-check-end-date');
        if (startDate.value && endDate.value != "" && startDate.value > endDate.value) {
            let temp = startDate.value;
            startDate.value = endDate.value;
            endDate.value = temp;
        }
        let url = new URL(location.href);
        if (startDate) {
            if (startDate.value && startDate.value != "") {
                url.searchParams.set('start-date', startDate.value);
            } else {
                url.searchParams.delete('start-date');
            }
            if (!endDate.classList.contains('d-none')) {
                url.searchParams.set('end-date', endDate.value);
            }
            if (endDate.value == "") {
                url.searchParams.delete('end-date');
            }
            location.replace(url);
        }
    }
}