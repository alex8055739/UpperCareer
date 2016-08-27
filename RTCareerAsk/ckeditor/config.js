///**
// * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
// * For licensing, see LICENSE.md or http://ckeditor.com/license
// */

//CKEDITOR.editorConfig = function( config ) {
//	// Define changes to default configuration here.
//	// For complete reference see:
//	// http://docs.ckeditor.com/#!/api/CKEDITOR.config

//	// The toolbar groups arrangement, optimized for two toolbar rows.
//	config.toolbarGroups = [
//		{ name: 'clipboard',   groups: [ 'clipboard', 'undo' ] },
//		{ name: 'editing',     groups: [ 'find', 'selection', 'spellchecker' ] },
//		{ name: 'links' },
//		{ name: 'insert' },
//		{ name: 'forms' },
//		{ name: 'tools' },
//		{ name: 'document',	   groups: [ 'mode', 'document', 'doctools' ] },
//		{ name: 'others' },
//		'/',
//		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
//		{ name: 'paragraph',   groups: [ 'list', 'indent', 'blocks', 'align', 'bidi' ] },
//		{ name: 'styles' },
//		{ name: 'colors' },
//		{ name: 'about' }
//	];

//	// Remove some buttons provided by the standard plugins, which are
//	// not needed in the Standard(s) toolbar.
//	config.removeButtons = 'Underline,Subscript,Superscript';

//	// Set the most common block elements.
//	config.format_tags = 'p;h1;h2;h3;pre';

//	// Simplify the dialog windows.
//	config.removeDialogTabs = 'image:advanced;link:advanced';
//};

CKEDITOR.editorConfig = function (config) {
    config.toolbarGroups = [
		{ name: 'links', groups: ['links'] },
		{ name: 'insert', groups: ['insert'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
    ];

    config.removeButtons = 'Subscript,Superscript,Cut,PasteText,Undo,Redo,PasteFromWord,Copy,Paste,Scayt,SpecialChar,Source,Underline,RemoveFormat,Anchor,Unlink,Outdent,Indent,Styles,Format,About';
    config.filebrowserImageUploadUrl = "/Home/UploadImage";

    config.extraPlugins = 'uploadimage,wordcount,notification';
    config.uploadUrl = '/Home/UploadDroppedAndPastedImage';
    config.wordcount = {
        showWordCount: false,
        showCharCount: true,
        maxCharCount: 1000
    };

    config.contentsCss = ['/Content/bootstrap.css', '/Content/Upper.css'];
};