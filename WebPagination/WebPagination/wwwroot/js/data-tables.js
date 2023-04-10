window.dataTables = [];

function DataTable(name, options, url) {

    let table = document.getElementById(name);

    table.className = 'data-table';

    let length = 10
    if (!(typeof options.pageLength === 'undefined' || options.pageLength === null)) {
        length = options.pageLength;
    }

    let tableSearchModel = {
        start: 0,
        length: length,
        order: [],
        columns: options.columns,
        search: {}
    };

    for (let c = 0; c < tableSearchModel.columns.length; c++) {
        let col = tableSearchModel.columns[c];
        if (!col.searchable) {
            col.searchable = true;
        }
        if (!col.orderable) {
            col.orderable = true;
        }
        if (!col.search) {
            col.search = {};
        }
    }

    var datatable = new DataTableObject(name, url, tableSearchModel);

    window.dataTables.push(datatable);

    FetchTable(datatable);

    var thead = table.getElementsByTagName('thead')[0];
    var header = thead.innerHTML;
    header = '<tr class="data-table-search"><th colspan="' + tableSearchModel.columns.length + '"><input type="text" placeholder="Search..." class="data-table-search-text" /></th></tr>' + header;
    thead.innerHTML = header;

    var tfoot = document.createElement('tfoot');
    tfoot.innerHTML = '<tr><th colspan="' + tableSearchModel.columns.length + '"><div class="data-table-description"></div><div class="data-table-nav"></div></th></tr>'
    table.appendChild(tfoot);

    var thead = table.getElementsByTagName('thead')[0].children[1];

    thead.addEventListener("click", function (e) {
        ReLoadTableSort(name, e.srcElement.cellIndex);
    }, false);

    var search = table.getElementsByClassName('data-table-search-text')[0];

    search.addEventListener("keyup", function (e) {
        ReLoadTableSearch(name, e.srcElement.value)
    }, false);

    return datatable;
}

function DataTableObject(name, url, model) {
    var Me = this;
    this.name = name;
    this.url = url;
    this.model = model;
    this.selectedItems = [];
    this.containsSelectedItem = function (id) {
        for (let s = 0; s < Me.selectedItems.length; s++) {
            if (Me.selectedItems[s] == id) {
                return true;
            }
        }
        return false;
    };
    this.getSelectedItems = function () {
        return Me.selectedItems;
    };
    this.removeSelectedItem = function (id) {
        const index = Me.selectedItems.indexOf(id);
        if (index > -1) {
            Me.selectedItems.splice(index, 1);
        }
    }
    this.addSelectedItem = function (id) {
        const index = Me.selectedItems.indexOf(id);
        if (index == -1) {
            Me.selectedItems.push(id);
        }
    }
}

function FetchTable(datatable) {

    fetch(datatable.url, {
        method: "POST",
        body: JSON.stringify(datatable.model),
        headers: { "Content-Type": "application/json" }
    })
        .then(response => response.json())
        .then(data => {
            datatable.model.recordsTotal = data.recordsTotal;
            LoadTable(datatable, data);
        });
}

function LoadTable(datatable, data) {

    let table = document.getElementById(datatable.name);

    var tBody = table.getElementsByTagName('tbody')[0];
    var desc = table.getElementsByClassName('data-table-description')[0];

    var len = datatable.model.start + datatable.model.length;
    if (len > datatable.model.recordsTotal) {
        len = datatable.model.recordsTotal;
    }

    desc.innerHTML = 'Showing ' + (datatable.model.start + 1) + ' to ' + len + ' of ' + datatable.model.recordsTotal + ' items'


    var nav = table.getElementsByClassName('data-table-nav')[0];
    var navInnerHtml = '<div onclick="ReLoadTablePage(\'' + datatable.name + '\',\'prev\')">Prev</div>';

    var currentPage = datatable.model.start / datatable.model.length;
    var startPage = currentPage - 3;
    if (startPage < 0) {
        startPage = 0;
    }
    while (
        ((startPage * datatable.model.length) <= datatable.model.recordsTotal)
        &&
        (startPage <= currentPage + 3)
    ) {
        navInnerHtml += '<div onclick="ReLoadTablePage(\'' + datatable.name + '\',' + (startPage + 1) + ')">' + (startPage + 1) + '</div>';
        startPage = startPage + 1;
    }

    navInnerHtml += '<div onclick="ReLoadTablePage(\'' + datatable.name + '\',\'next\')">Next</div>';
    nav.innerHTML = navInnerHtml;

    var cols = datatable.model.columns;

    var st = '';

    for (let r = 0; r < data.data.length; r++) {

        let item = data.data[r];

        st += '<tr>';

        for (let c = 0; c < cols.length; c++) {
            st += RenderColValue(datatable, cols[c], item);
        }

        st += '</tr>';
    }

    tBody.innerHTML = st;
}

function RenderColValue(datatable, column, item) {
    let className = 'data-table-item';
    let colstring = '';

    if (column.dataType) {
        className += '-' + column.dataType;
    }

    if (column.dataType == "select") {
        let selected = '';
        if (datatable.containsSelectedItem(item[column.data])) {
            selected = ' checked="checked" ';
        }
        colstring += '<input type="checkbox"' + selected + ' onchange="SetSelected(\'' + datatable.name + '\', \'' + item[column.data] + '\', this.checked )"/>';

    } else if (column.dataType == "date") {

        var dt = new Date(item[column.data]);

        var mnth = (dt.getMonth() + 1).toString();
        if (mnth.length == 1) { mnth = "0" + mnth; }

        var day = dt.getDate().toString();
        if (day.length == 1) { day = "0" + day; }

        colstring += dt.getFullYear() + "-" + mnth + "-" + day;

    } else if (column.dataType == "currency") {
        colstring += FormatFloat(item[column.data], ' ');
    } else {
        if (column.render) {
            colstring += column.render(item[column.data], 'display', item);
        } else {
            colstring += item[column.data];
        }
    }

    colstring = '<td class="' + className + '">' + colstring + '</td>';

    return colstring;
}

function SetSelected(name, value, checked) {
    var dt = GetDataTable(name);
    if (checked == true) {
        dt.addSelectedItem(value);
    } else {
        dt.removeSelectedItem(value);
    }
}

function FormatFloat(f, thousandsChar) {

    if (thousandsChar == null) { thousandsChar = ' '; }

    var st = parseFloat(f).toFixed(2).toString().trim().replace(' ', '');
    var rtn = st.substring(st.length - 3, st.length);
    st = st.replace(rtn, '');
    while (st.length > 0) {
        if (st.length >= 3) {
            rtn = st.substring(st.length - 3, st.length) + thousandsChar + rtn;
            st = st.substring(0, st.length - 3);
        } else {
            rtn = st + thousandsChar + rtn;
            st = '';
        }
    }

    return rtn.replace('- ', '-').replace(' .', '.');
}

function GetDataTable(name) {
    for (let t = 0; t < window.dataTables.length; t++) {
        if (window.dataTables[t].name == name) {
            return window.dataTables[t];
        }
    }
}

function ReLoadTableSearch(name, value) {

    var dt = GetDataTable(name);

    dt.model.search = { value: value };

    let timer = setTimeout(function () {
        clearTimeout(timer);
        FetchTable(dt);
    }, 300);
}

function ReLoadTableSort(name, colindex) {

    var dt = GetDataTable(name);

    let order = null;

    for (let s = 0; s < dt.model.order.length; s++) {
        if (dt.model.order[s].column == colindex) {
            order = dt.model.order[s];
        }
    }

    if (order) {
        if (order.dir == "asc") {
            order.dir = "desc"
        } else {
            order.dir = "asc"
        }
    } else {
        dt.model.order.push({ column: colindex, dir: "asc" });
    }

    let timer = setTimeout(function () {
        clearTimeout(timer);
        FetchTable(dt);
    }, 100);
}

function ReLoadTablePage(name, page) {
    var dt = GetDataTable(name);

    let currentStart = dt.model.start;
    let start = 0;

    if (page == 'prev') {
        start = currentStart - dt.model.length;
        if (start < 0) {
            start = 0;
        }
    } else if (page == 'next') {
        start = currentStart + dt.model.length;
        if (start > dt.model.recordsTotal) {
            start = currentStart;
        }
    } else {
        start = (page - 1) * dt.model.length;
        if (start > dt.model.recordsTotal) {
            start = currentStart;
        }
    }

    dt.model.start = start;

    let timer = setTimeout(function () {
        clearTimeout(timer);
        FetchTable(dt);
    }, 100);
}