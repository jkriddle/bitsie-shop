/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.extraPlugins = 'dialog,base64image';
    config.allowedContent = true;
    config.toolbar = [
		{
		    name: 'document', groups: ['mode', 'document', 'doctools'],
		    items: ['Source']
		},
	    {
	        name: 'clipboard', groups: ['clipboard', 'undo'],
	        items: ['Cut', 'Copy', 'Paste', 'PasteFromWord', '-', 'Undo', 'Redo']
	    },
	    {
	        name: 'basicstyles', groups: ['basicstyles', 'cleanup'],
	        items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat']
	    },
	    {
	        name: 'insert',
	        items: ['base64image', 'HorizontalRule']
	    },
	    {
	        name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'],
	        items: ['NumberedList', 'BulletedList', '-', 'Blockquote', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock']
	    },
	    {
	        name: 'links',
	        items: ['Link', 'Unlink', 'Anchor']
	    },
	    {
	        name: 'styles',
	        items: ['Font', 'FontSize']
	    },
	    {
	        name: 'colors',
	        items: ['TextColor', 'BGColor']
	    }
    ];
};
