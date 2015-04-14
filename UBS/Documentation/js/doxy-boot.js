$( document ).ready(function() {

	$("div.headertitle").addClass("page-header");
	$("div.title").addClass("h1");

	$("img").addClass("img-responsive");
	
	$('li > a[href="index.html"] > span').before("<i class='fa fa-cog'></i> ");
	$('li > a[href="index.html"] > span').text("Getting started");
	$('li > a[href="modules.html"] > span').before("<i class='fa fa-square'></i> ");
	$('li > a[href="namespaces.html"] > span').before("<i class='fa fa-bars'></i> ");
	$('li > a[href="annotated.html"] > span').before("<i class='fa fa-list-ul'></i> ");
	$('li > a[href="classes.html"] > span').before("<i class='fa fa-book'></i> ");
	$('li > a[href="inherits.html"] > span').before("<i class='fa fa-sitemap'></i> ");
	$('li > a[href="functions.html"] > span').before("<i class='fa fa-list'></i> ");
	$('li > a[href="functions_func.html"] > span').before("<i class='fa fa-list'></i> ");
	$('li > a[href="functions_vars.html"] > span').before("<i class='fa fa-list'></i> ");
	$('li > a[href="functions_enum.html"] > span').before("<i class='fa fa-list'></i> ");
	$('li > a[href="functions_eval.html"] > span').before("<i class='fa fa-list'></i> ");
	$('img[src="ftv2ns.png"]').replaceWith('<span class="label label-danger">N</span> ');
	$('img[src="ftv2cl.png"]').replaceWith('<span class="label label-danger">C</span> ');
	
	$("ul.tablist").addClass("nav nav-pills");

	$("ul.tablist").css({ margin: "0.5em auto"});
	$("li.current").addClass("active");
	$("iframe").attr("scrolling", "yes");
	
	$("#nav-path > ul").addClass("breadcrumb");
	
	$("table.params").addClass("table");
	$("div.ingroups").wrapInner("<small></small>");
	$("div.levels").css("margin", "0.5em");
	$("div.levels > span").addClass("btn btn-default btn-xs");
	$("div.levels > span").css("margin-right", "0.25em");
	
	$("table.directory").addClass("table table-striped");
	$("div.summary > a").addClass("btn btn-default btn-xs");
	$("table.fieldtable").addClass("table");
	$(".fragment").addClass("well");
	$(".memitem").addClass("panel panel-default");
	$(".memproto").addClass("panel-heading");
	$(".memdoc").addClass("panel-body");
	$("span.mlabel").addClass("label label-info");
	
	$("table.memberdecls").addClass("table");
	$("[class^=memitem]").addClass("active");
	
	$("div.ah").addClass("btn btn-default");
	$("span.mlabels").addClass("pull-right");
	$("table.mlabels").css("width", "100%")
	$("td.mlabels-right").addClass("pull-right");

	$("div.ttc").addClass("panel panel-info");
	$("div.ttname").addClass("panel-heading");
	$("div.ttdef,div.ttdoc,div.ttdeci").addClass("panel-body");
	$("span.icon").addClass("label label-danger");
	
});