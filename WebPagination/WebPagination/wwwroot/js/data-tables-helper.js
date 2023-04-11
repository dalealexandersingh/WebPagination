function getDataTableDefaultOptions(url) {
    var options = {};

    options.ajax = {
        url: url,
        cache: false,
        type: "POST"
    };

    options.pageLength = 10;
    options.dom = 'ftipr';
    options.processing = true;
    options.serverSide = true;
    options.responsive = true;
    options.searching = true;
    options.paging = true;
    options.select = true;

    return options;
}

(function ($) {

    $.fn.Table = function (options) {

        let tbl = this.DataTable(options);

        tbl.selectedItems = [];

        tbl.TestName = function () {
            alert(this.test);
        };

        tbl.containsSelectedItem = function (id) {
            for (let s = 0; s < tbl.selectedItems.length; s++) {
                if (tbl.selectedItems[s] == id) {
                    return true;
                }
            }
            return false;
        };

        tbl.removeSelectedItem = function (id) {
            const index = tbl.selectedItems.indexOf(id);
            if (index > -1) {
                tbl.selectedItems.splice(index, 1);
            }
        };

        tbl.addSelectedItem = function (id) {
            const index = tbl.selectedItems.indexOf(id);
            if (index == -1) {
                tbl.selectedItems.push(id);
            }
        };

        tbl.setSelected = function (value, checked) {
            if (checked == true) {
                tbl.addSelectedItem(value);
            } else {
                tbl.removeSelectedItem(value);
            }
        }

        return tbl;
    };

}(jQuery));

function selectAll(event) {
    let checkbox = event.srcElement;
    let index = checkbox.parentElement.cellIndex;
    var table = checkbox.parentElement.parentElement.parentElement.parentElement;
    var body = table.getElementsByTagName('tbody')[0];
    for (var r = 0; r < body.children.length; r++) {
        let check = body.children[r].children[index].children[0];
        check.checked = checkbox.checked;
        check.dispatchEvent(new Event("change"));
    }
}

function formatDate(date) {

    var dt = new Date(date);

    var mnth = (dt.getMonth() + 1).toString();
    if (mnth.length == 1) { mnth = "0" + mnth; }

    var day = dt.getDate().toString();
    if (day.length == 1) { day = "0" + day; }

    return dt.getFullYear() + "-" + mnth + "-" + day;
}

function formatFloat(f, thousandsChar) {

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