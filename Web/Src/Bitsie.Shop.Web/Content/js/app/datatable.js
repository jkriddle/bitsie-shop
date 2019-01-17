/**
 * Dynamic paged data table that integrates with the API
 * to display results, render a pager, and accommodate searches and sorting.
 */

// Basic search filter that can be extended for additional queries
// or filters. Additional properties are added dynamically by
// the advanced search form in the DataTable class below.
var SearchFilter = BaseObject.extend({
    init: function (opts) {
        this._super(opts);
        this.SearchTerm = "";
        this.NumPerPage = 20;
        this.CurrentPage = 1;
        this.SortDirection = "Ascending";   // Or "Descending"
        this.SortColumn = "";
    }
});

// Data table object that encompasses all functionality
var DataTable = Class.extend({

    // jQuery object
    _table: null,

    init: function (table, filter, obj, advSearch) {
        var self = this;

        self._table = $(table);
        self.sortDirection = "Ascending";
        self.sortColumn = "";

        // Sort
        $('th a', self._table).click(function (e) {
            e.preventDefault();
            filter.SortColumn = this.hash.slice(1);

            // Clicked on same sort column
            if (filter.SortColumn == self.sortColumn) {
                // Switch directions
                filter.SortDirection = self.sortDirection == "Ascending" ? "Descending" : "Ascending";
            } else {
                // Default to ascending
                filter.SortDirection = "Ascending";
            }
            // Store new values
            self.sortColumn = filter.SortColumn;
            self.sortDirection = filter.SortDirection;
            obj.get(filter);
        });

        // Num per page selection
        $('select').select2({ minimumResultsForSearch: 11 }).on('change', function (e) {
            filter.NumPerPage = e.val;
            filter.CurrentPage = 1;
            obj.get(filter);
        });

        // Search term change
        $('.searchTerm', self._table).change(function () {
            var val = $(this).val();
            filter.SearchTerm = val;
            filter.CurrentPage = 1;
            obj.get(filter);
        });

        // Pagination
        $('.table-pager a', self._table).live('click', function (e) {
            filter.CurrentPage = this.hash.slice(1);
            obj.get(filter);
            e.preventDefault();
        });

        // Advanced search
        $('.advanced', self._table).click(function (e) {
            e.preventDefault();
            var html = Bitsie.Shop.Template('#searchTemplate', filter);
            bootbox.alert(html, function () {
                // Copy parameters over to filter and do search
                var formData = $('.modal-body form').serializeObject();
                filter = $.extend({}, filter, formData);
                console.log(filter);
                obj.get(filter);
            });
            if (advSearch != undefined) advSearch();
        });

        // Export
        $('.export', self._table).click(function (e) {
            e.preventDefault();
            obj.export(filter);
        });

        // Load initial data
        obj.get(filter);
    },

    // Update the pager to reflect a new page display
    update: function (currentPage, numPages) {
        // Determine which pages should be shown
        var maxPagesToShow = 10;
        var numToShowDiff = Math.floor(maxPagesToShow / 2);

        var start = currentPage - numToShowDiff;
        var end = (start + (numToShowDiff * 2)) - 1;

        var diff = 0;
        if (start < 1) {
            diff = 1 - start;
            start = 1;
            end += diff;
        }

        if (end > numPages) {
            diff = end - numPages;
            end = numPages;
            start -= diff;
        }

        if (start < 1) start = 1;

        // Create array of pages for use by mustache template
        var pages = [];
        for (var i = start; i <= end; i++) {
            pages.push(i);
        }

        // Object used for Handlebars rendering
        var pg = {
            FirstClass: '',
            FirstPage: 1,
            NextPage: (currentPage == numPages ? currentPage : currentPage + 1),
            NextClass: '',
            PrevPage: (currentPage == 1 ? 1 : currentPage - 1),
            PrevClass: '',
            CurrentPage: currentPage,
            LastPage: numPages,
            LastClass: '',
            Pages: pages
        };

        if (pg.CurrentPage == 1) {
            pg.FirstClass = 'ui-state-disabled';
            pg.PrevClass = 'ui-state-disabled';
        }

        if (pg.CurrentPage == pg.LastPage || numPages == 0) {
            pg.LastClass = 'ui-state-disabled';
            pg.NextClass = 'ui-state-disabled';
        }

        var template = '<a class="first ui-button {{FirstClass}}" href="#{{FirstPage}}">First</a><a class="previous ui-button {{PrevClass}}" href="{{PrevPage}}">Previous</a><span>{{#Pages}}<a class="ui-button page-{{.}}" href="#{{.}}">{{.}}</a>{{/Pages}}</span><a tabindex="0" class="next ui-button {{NextClass}}" href="#{{NextPage}}">Next</a><a tabindex="0" class="last ui-button {{LastClass}}" href="#{{LastPage}}">Last</a>';
        var html = Bitsie.Shop.Template.raw(template, pg);
        $('.table-pager', self._table).html(html);
        $('.page-' + pg.CurrentPage, self._table).addClass('ui-state-disabled');
    },

    /* Display "no records found" message */
    empty: function (message) {
        if (!message) message = "No records found.";
        var numCols = $('thead th', self._table).length;
        var tr = $('<tr><td colspan="' + numCols + '">' + message + '</td></tr>');
        $('tbody', self._table).html(tr);
    }

});