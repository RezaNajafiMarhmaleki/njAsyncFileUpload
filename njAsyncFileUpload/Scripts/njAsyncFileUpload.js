; (function ($, window, document, undefined) {
    "use strict";
    // Default options
    var defaults = {
        paths: {
            listsfiles: '/njAsyncFileUpload/',
            addfiles: '/njAsyncFileUpload/Add',
            removefiles: '/njAsyncFileUpload/Delete',
        },
        requireImage: true,
        maxFilesize: 4,
    };

    // The plugin constructor
    function njAsyncFileUpload(element, options) {
        // Merge user settings with default, recursively
        this.file_element = $(element);

        this.options = $.extend(true, {}, defaults, options);

        // Call initial method
        this.init();
    }

    $.extend(njAsyncFileUpload.prototype, {
        init: function () {
            this.Refresh();
        }, 
        _njcreatefilerowhtml: function (value) {
            var sc = this;
            var li = $("<li/>", {
                "class": "list-group-item  list-group-item-info",
                "style": "padding:15px 15px 15px 15px",
                id: "file_item" + value.Id,
                html:'',
            });
            var row = $("<div/>", {
                "class": "row",
                html: (value.IsSuccessful ? value.FileName + "  <strong>" + Number.parseFloat(value.FileSize / 1024).toFixed(2) + 'kb </strong>' : value.FileName + ' :<strong>' + value.Message + '</strong>')
            });
            var deletebutton = $("<button/>", {
                "class": "btn btn-default btn-sm pull-left",
                "type": "button",
                "id": value.Id,
                html: '<span class="glyphicon glyphicon-trash"></span>',
            });
            deletebutton.click(function () {
                sc.Remove(value.Id,li);
            });
            deletebutton.appendTo(row);
            row.appendTo(li);
            return li;
        },
        _njsendfile: function (file) {
            var sc = this;
            var data = new FormData();
            var iSize = (file.size / 1024);
            iSize = (Math.round((iSize / 1024) * 100) / 100);

            if (iSize >= sc.options.maxFilesize) {
                alert("فایل " + file.name + " باید کمتر از " + sc.options.maxFilesize + " مگابایت باشد");
                return;
            }
            data.append("postedfile", file);
            data.append("RequireImage", sc.options.requireImage);

            var li = $("<li/>", {
                "class": "list-group-item  list-group-item-info",
                "style": "padding:15px 15px 15px 15px",
                html:'',
            });
            var row = $("<div/>", {
                "class": "row",
                html: file.name + "  <strong>" + Number.parseFloat(file.size / 1024).toFixed(2) + 'kb </strong>',
            });
            var infolable = $("<div/>", {
                "class": "label label-info",
                "type": "button",
                html: '<span class="glyphicon glyphicon-trash"></span>',
            });
            infolable.appendTo(row);
            var cancelbutton = $("<button/>", {
                "class": "btn btn-default btn-sm pull-left",
                "type": "button",
                html: '<span class="glyphicon glyphicon-remove"></span>',
            });
            cancelbutton.appendTo(row);
            row.appendTo(li);
            $('#njAsyncFileUpload').append(li);
            $.ajax({
                type: "POST",
                url: sc.options.paths.addfiles,
                contentType: false,
                processData: false,
                data: data,
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    xhr.upload.addEventListener("progress", function (evt) {                                
                        cancelbutton.click(function () {
                            xhr.abort();
                        });
                        if (evt.lengthComputable) {
                            infolable.html('بارگزاری... ' + Math.round((evt.loaded / evt.total) * 100) + '%');
                        }
                        else infolable.html('hmmm');
                    }, false);
                    return xhr;
                },
                success: function (result) {
                    li.fadeOut('slow');
                    if (result.Result != 'OK') { alert(result.Message); return; }
                    $.each(result.Records, function (key, value) {                        
                        $('#njAsyncFileUpload').append(sc._njcreatefilerowhtml(value));
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    li.fadeOut('slow');
                    $("#_njinputfile").val('');
                    alert("فایل ارسال نشد. " + xhr.responseText);
                }
            });
        },
        Remove: function (id,tag) {
            $.post(this.options.paths.removefiles, { id: id }, function (result) {
                if (result.Result != 'OK')
                    alert(result.Message);
                else
                    tag.fadeOut('slow');
            });
        },
        Add: function () {
            var files = $("#_njinputfile").get(0).files;
            var sc = this;
            if (files.length == 0) {
                alert("لطفا فایلی را جهت ارسال انتخاب کنید");
                return;
            }
            if (window.FormData != undefined) {
                for (var i = 0; i < files.length; i++) {                    
                    sc._njsendfile(files[i]);
                }
                $("#_njinputfile").val('');

            } else {
                alert("Your browser doesn't support HTML5 multiple file uploads! Please use some decent browser.");
            }

        },
        Refresh: function () {
            var sc = this;
            $.post(this.options.paths.listsfiles, {}, function (result) {
                //sc.files = result.Records;
                if (result.Result != 'OK') { alert(result.Message); return; }
                var njrow = $('<div>', {
                    "class": "row",
                    "id": "_njfileuploadhrow",
                });
                njrow.html('<input type="file" id="_njinputfile" name="postedfile" multiple="multiple" class="control-label col-md-6"/>');
                var uploadbutton = $("<button/>", {
                    "class": "btn btn-default btn-sm col-md-2",
                    "type": "button",
                    "id": "_njfileuploadbutton",
                    html: '<span class="glyphicon glyphicon-upload"> آپلود</span>',
                });
                uploadbutton.click(function () {
                    sc.Add();
                });
                uploadbutton.appendTo(njrow);
                sc.file_element.html(njrow);

                //body
                var ul = $("<ul/>", {
                    "class": "list-group",
                    "id": "njAsyncFileUpload"
                });
                $.each(result.Records, function (key, value) {
                   ul.append(sc._njcreatefilerowhtml(value));
                });


                sc.file_element.append(ul);


            });
        },
    });
    $.fn.njAsyncFileUpload = function (options) {
        var args = arguments;
        var instance;

        if (options === undefined || typeof options === 'object') {
            return this.each(function () {
                if (!$.data(this, "njAsyncFileUpload")) {
                    $.data(this, "njAsyncFileUpload", new njAsyncFileUpload(this, options));
                }
            });
        } else if (typeof options === 'string' && options[0] !== '_' && options !== 'init') {
            instance = $.data(this[0], 'njAsyncFileUpload');

            if (options === 'destroy') {
                $.data(this, 'njAsyncFileUpload', null);
            }

            if (options === 'add') {

                instance.Add(args[1], args[2]);
            }
            if (options === 'remove') {

                instance.Remove(args[1]);
            }

            if (instance instanceof njAsyncFileUpload && typeof instance[options] === 'function') {
                return instance[options].apply(instance, Array.prototype.slice.call(args, 1));
            } else {
                return this;
            }
        }
    };

})(jQuery, window, document);


