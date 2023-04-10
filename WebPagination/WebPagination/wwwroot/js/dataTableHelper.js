
function SetGridOptions(options) {

    if (typeof options.deferRender === "undefined") {
        options.deferRender = true;
    }

    if (typeof options.responsive === "undefined") {
        options.responsive = true;
    }

    if (typeof options.searching === "undefined") {
        options.searching = true;
    }

    if (typeof options.paging === "undefined") {
        options.paging = true;
    }

    if (typeof options.orderCellsTop === "undefined") {
        options.orderCellsTop = true;
    }

    if (typeof options.fixedHeader === "undefined") {
        options.fixedHeader = true;
    }

    if (typeof options.pageLength === "undefined") {
        options.pageLength = 300;
    }

    if (typeof options.scrollCollapse === "undefined") {
        options.scrollCollapse = true;
    }

    if (typeof options.processing === "undefined") {
        options.processing = true;
    }

    if (typeof options.dom === "undefined") {
        options.dom = '<"top"fi>tpr';
    }

    if (typeof options.initComplete === "undefined") {
        options.initComplete = function () {
            $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
        }
    }

    if (typeof options.drawCallback === "undefined") {
        options.drawCallback = function () {
            $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
        }
    }

    if (typeof options.language === "undefined") {
        options.language = {
            'processing': '<div class="loader"></div>'
        }
    }

    if (typeof options.serverSide === "undefined") {
        options.serverSide = false;
    }

    return options;
}

_currencySort = function (a, b) {
    a = a.replace(/<[^>]*>?/gm, ''); //ignoring tags
    b = b.replace(/<[^>]*>?/gm, ''); //ignoring tags
    a = parseFloat(a.replace(/\s/g, "").toUpperCase().replace("R", "").replace("$", "").trim());
    b = parseFloat(b.replace(/\s/g, "").toUpperCase().replace("R", "").replace("$", "").trim());
    return ((a < b) ? -1 : ((a > b) ? 1 : 0));
}

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "currencysort-asc": function (a, b) {
        return _currencySort(a, b);
    },
    "currencysort-desc": function (a, b) {
        return _currencySort(a, b) * -1;
    }
});

_simplesearch = function (sData) {
    return sData.replace(/,/g, "").replace(/\s+/g, "");
}

jQuery.extend(jQuery.fn.dataTableExt.ofnSearch, {
    "currencysort": function (a) {
        return _simplesearch(a);
    }
});

jQuery.fn.dataTable.Api.register('processing()', function (show) {
    return this.iterator('table', function (ctx) {
        ctx.oApi._fnProcessingDisplay(ctx, show);
    });
});

(function ($) {

    $.fn.Table = function (options, url) {

        SetGridOptions(options, url);

        var id = this.attr('id');

        var winHeight = window.outerHeight;
        var gridheight = $("#" + id).offset().top;
        var scrollHeight = winHeight - gridheight - 300; //the 300 is an estimate of the table header
        if (scrollHeight < 400) {
            scrollHeight = 400; //set the grid scroll to a minimum
        }

        AddGridSearchOptions(id, options, url);

        options.scrollY = function () {
            return scrollHeight + 'px';
        }

        var tbl = this.DataTable(options);

        if (options.serverSide == false) {
            AlignGridColumns(tbl);
        }

        tbl.on('xhr.dt', function () {
            AlignGridColumns(tbl);
        });

        return tbl;
    };

}(jQuery));

function AlignGridColumns(tbl, time) {
    if (time == null) { time = 500; }
    setTimeout(function () {
        tbl.columns.adjust();
    }, time);
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

function AddGridSearchOptions(elementName, options, url) {

    var st = '<tr>';

    $('#' + elementName + ' thead tr th').each(function () {
        st += '<th></th>'
    });

    st += '</tr>';

    $('#' + elementName + ' thead').append(st);

    var headerCols = $('#' + elementName + ' thead tr:eq(1)').children();

    var cols = [];
    var colValues = [];
    var colsDD = [];
    if (options.columns != null) {

        for (i = 0; i < options.columns.length; i++) {
            var column = options.columns[i];
            column.searchable = false;
            var colvalue = "";
            
            if (column.searchType == "text") {
                cols.push(i);
                column.searchable = true;
            }
            if (column.searchType == "dropdown") {
                colsDD.push(i);
                column.searchable = true;
            }
            
            if (column.searchValue != null) {
                colvalue = column.searchValue;
            }

            if (column.dataType == "date") {
                column.render = function (data, type, item) {
                    var dt = new Date(data);
                    var st = "";
                    st += dt.getFullYear() + "-";

                    var mnth = (dt.getMonth() + 1).toString();
                    if (mnth.length == 1) { mnth = "0" + mnth; }
                    st += mnth + "-";

                    var day = dt.getDate().toString();
                    if (day.length == 1) { day = "0" + day; }
                    st += day;

                    return st;
                }
            }

            if (column.dataType == "currency") {
                column.render = function (data, type, item) {
                    
                    var st = "";
                    st += "R " + FormatFloat(data, ' ');

                    return st;
                }
            }
            
            colValues.push(colvalue);
        }
    }

    //setting initial search values
    if (url != null) {
        options.ajax = {
            url: url,
            cache: false,
            type: "POST",
            data: function (d) {
                if (colValues != null && d.columns != null) {
                    for (c = 0; c < d.columns.length; c++) {
                        var colValue = colValues[c];
                        if (colValue != null) {
                            d.columns[c].search.value = colValue;
                        }
                    }
                }
                if (options.parameters != null) {
                    for (var key in options.parameters) {
                        d[key] = eval(options.parameters[key]);
                    }
                }
            },
            dataSrc: function (jsn) {
                return jsn.data;
            }
        };
        options.serverSide = true;
    }

    options.initComplete = function () {

        this.api().columns(cols).every(function () {
            var column = this;

            var colvalue = "";
            var colNo = column["0"];
            if (colValues != null) {
                if (colValues.length >= (colNo - 1)) {
                    colvalue = colValues[colNo];
                }
            }

            $('<input class="form-control grid-search" type="search" placeholder="Search..." value="' + colvalue + '" />')
                .appendTo($(headerCols[colNo]).empty())
                .on('keyup change clear', function () {
                    if (column.search() !== this.value) {
                        column.search(this.value).draw();
                    }
                })
                .on('input propertychange', function () {
                    if ((column.search() !== this.value) && (this.value == "")) {
                        column.search(this.value).draw();
                    }
                });

            column.search(colvalue).draw();
        });

        this.api().columns(colsDD).every(function () {
            var column = this;

            var colvalue = "";
            var colNo = column["0"];
            if (colValues != null) {
                if (colValues.length >= (colNo - 1)) {
                    colvalue = colValues[colNo];
                }
            }

            var select = $('<select class="form-control grid-select"><option value=""></option></select>')
                .appendTo($(headerCols[column["0"]]).empty())
                .on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );
                    column.search(val ? '^' + val + '$' : '', true, false).draw();
                });

            column.data().unique().sort().each(function (d, j) {
                select.append('<option value="' + d + '">' + d + '</option>')
            });

            column.search(colvalue).draw();
        });

        $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
        colValues = null;

    }

}