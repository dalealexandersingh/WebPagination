# WebPagination

Summary:
	.net 6 web mvc application to demonstrate custom web grid pagination with sorting and filtering server side.
	Simply build a query on your dbcontext and use the custom QueryableExtensions to return the paginated data (profile the database to view the executed scripts!)
	
Databases:
	Example database is SQLite, however MSSQL can be used (preferred).
	Modify implementation of DBContext to use any implementation.

Dependencies:
	JQuery
	JQuery datatables: https://datatables.net/
	These are used in the example index.html file

Custom:
	DataTableHelper: helper classes to transform and modify jquery datatables
	QueryableExtensions: Custom extension to 