(function ($) {
    $.fn.editableSelect = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method: ' + method + ' does not exist in jQuery.editableSelect.js');
        }
    }

    $.fn.editableUpdate = function () {
        return methods.update.apply(this, arguments);
    }

    $.fn.editableSelect.defaults = {
        enableiframe: false,
        textWidth: null
    };

    var methods = {
        init: function (options) {
            var settings = $.fn.editableSelect.defaults;
            if (options) {
                $.extend(settings, options);
            }
            return $(this).each(function create() {
                var $element = $(this);
                $element.data('editableSelect', {
                    editableSelect: new EditableSelect($element, settings)
                });
            });
        },
        update: function () {
            return $(this).each(function update() {
                var $instance = getInstance(this);
                $instance.$container.contents().remove();
                $instance.buildList();
            });
        },
        enable: function () {
            return this.each(function enable() {
                getInstance(this).enable();
            });
        },
        disable: function () {
            return this.each(function disable() {
                getInstance(this).disable();
            });
        }
    };

    function getInstance(element) {
        var data = $(element).data('editableSelect');
        if (data) {
            return data.editableSelect;
        }
        $.error('editableSelect not initialized.');
    };

    function EditableSelect($element, options) {
        this.init($element, options);
    };

    EditableSelect.prototype = {
        $textbox: {},
        $select: {},
        $container: {},
        $iframe: false,
        $listItems: false,
        value: '',
        index: -1,
        liValues: {},
        options: {},
        init: function ($element, options) {
            this.options = options;
            this.$select = $element;
            var $clone = this.$select.clone(true);
            $clone.css({ display: 'block', top: '-1000' });
            $clone.appendTo('body');
            this.$container = $('<div class="esContainer" />').appendTo(document.body);
            this.$textbox = $('<input type="text" class="esTextBox">').css({
                //width: $clone.width(),//$clone.outerWidth(),
                height: $clone.outerHeight()
            }).attr('tabindex', this.$select.attr('tabindex'));

            if (this.options.textWidth) {
                this.$textbox.width(this.options.textWidth);
            }
            else {
                this.$textbox.width($clone.outerWidth());
            }
            this.$textbox.appendTo(this.$select.parent());
            this.buildList();
            this.$select.hide();
            $clone.remove();
            this._hideList();
            this._bindEvents();
            if (this.$select.is(':disabled')) {
                this.disable();
            }
            this.tester = true;
        },
        enable: function () {
            this.$textbox.removeAttr('disabled');
        },
        disable: function () {
            this.$textbox.attr('disabled', 'disabled');
        },
        buildList: function () {
            var self = this;
            var selected = false;
            var $list = $('<ul>').css({
                'list-style-type': 'none',
                padding: 0,
                margin: 0
            }).appendTo(self.$container).addClass('noEdit');
            self.liValues = [];
            self.$listItems = $(self.$select.children('option').map(function buildOptions(idx, element) {
                var $element = $(element);
                var text = $element.text();
                if ($element.is(':selected')) {
                    self.value = text;
                    selected = true;
                }
                var li = $('<li/>', { text: text });
                if (selected) {
                    self.index = idx;
                    selected = false;
                }
                self.liValues.push(text);
                return li.get();
            })).appendTo($list);
            self.$listItems.wrap('<div class="esItem" />');
            self._getCurrentLiDiv().addClass('esItemHover');
            self.$textbox.val(self.value);
        },
        refreshList: function (inputWord) {
            var self = this;
            var $div = self.$container;
            var visible = $div.is(':visible');
            self.$listItems.each(function (idx, element) {
                var $element = $(element);
                var text = $element.text();
                if (!inputWord || inputWord == '' || text.toUpperCase().indexOf(inputWord.toUpperCase()) >= 0) {
                    $element.show();
                }
                else {
                    $element.hide();
                }
            });
            self._showList();
        },
        _bindEvents: function () {
            var self = this;
            this.$textbox.click(function tbClick(e) {
                e.stopPropagation();
                if (self.$container.is(':visible')) {
                    self._hideList();
                } else {
                    $(document.body).trigger('jqes.closeAll');
                    self.refreshList();
                    $(this).select();
                }
            }).keyup(function tbKeyup(e) {
                var visible = self.$container.is(':visible');
                switch (e.keyCode) {
                    case 9://Tab
                    case 13://Enter
                        if (visible) {
                            self._selectItem(self._getCurrentLiDiv().text());
                        }
                        if (e.keyCode == 13) {
                            e.preventDefault();
                            $(this).trigger('change');
                            return false;
                        }
                        break;
                    case 38://Up
                        if (visible) {
                            e.preventDefault();
                            self._moveSelection('up');
                        }
                        else {
                            self._showList();
                        }
                        break;
                    case 40://Down
                        if (visible) {
                            e.preventDefault();
                            self._moveSelection('down');
                        } else {
                            self._showList();
                        }
                        break;
                    case 27://Esc
                        e.preventDefault();
                        self.$textbox.val(self.value);
                        self._cancelSelection();
                        break;
                    default:
                        break;
                }
            });

            this.$textbox.bind('input propertychange', function () {
                var textVal = self.$textbox.val();
                self.refreshList(textVal);
            }).blur(function () {
                self.$textbox.val(self.value);
                //self._getCurrentLiDiv().addClass('esItemHover');
            });;
            $(document.body).bind('click.jqes jqes.closeAll', function bodyClickClose(e) {
                if (e.target !== self.$textbox.get(0) && !self.$container.has(e.target).length) {
                    self._cancelSelection();
                }
            });
            self.$container.on("mouseup", "li", function liMouseUp(e) {
                if (e.target === this) {
                    self._selectItem($(this).text());
                    e.stopPropagation();
                }
            }).on("hover", "li", function lihover() {
                self._getCurrentLiDiv().removeClass('esItemHover');
                $(this).toggleClass('esItemHover');
            });
        },
        _showList: function () {
            if (this.$listItems.length === 0) {
                return;
            }
            if (this.$container.is(':hidden')) {
                this._getCurrentLiDiv().addClass('esItemHover');
            }
            this.$container.show();
            var viewHeight = $(window).height();
            var scrollTop = $(window).scrollTop();
            var textHeight = this.$textbox.outerHeight();
            var textWidth = this.$textbox.outerWidth();
            this.$container.css({
                'width': textWidth
            });
            this.$container.find('ul').css({
                width: '100%'
            });
            if (this.$select.outerHeight() > this.$textbox.outerHeight()) {
                textHeight = parseInt(this.$select.outerHeight());
            }
            //var itemsHeight = this.$listItems.length * $(this.$listItems[0]).parent().outerHeight();
            var itemsHeight = 0;
            var isHigher = false;
            for (var i = 0; i < this.$listItems.length; i++) {
                var iHight = parseInt(this.$listItems.eq(i).parent().outerHeight())
                itemsHeight += iHight;
                if (iHight > textHeight) {
                    isHigher = true;
                }
            }

            var offset = this.$textbox.offset();
            var rightWidth = $(window).width() - offset.left - textWidth;
            if (isHigher) {
                if (rightWidth > 150) {
                    textWidth = textWidth + 130;
                }
                else if (rightWidth < 150 && rightWidth > 20) {
                    textWidth = textWidth + rightWidth - 20;
                }
            }
            this.$container.css({
                'width': textWidth
            });
            itemsHeight = this.$container.find("ul").outerHeight();
            //itemsHeight = this.$container.outerHeight();
            var containerHeight = itemsHeight + 2;
            var spaceUp = offset.top - scrollTop;
            var spaceDown = viewHeight - (spaceUp + textHeight);
            var verticalMax = spaceUp > spaceDown ? spaceUp : spaceDown;
            if (itemsHeight > verticalMax) {
                containerHeight = verticalMax;
            }
            this.$container.css({
                'height': containerHeight
            });
            if ((spaceUp > spaceDown) && (containerHeight > spaceDown)) {
                offset.top = offset.top - containerHeight;
            } else {
                offset.top = offset.top + textHeight;
            }
            this.$container.css({
                'min-width': this.$textbox.outerWidth(),
                'overflow-y': function overflow() {
                    return itemsHeight > containerHeight ? 'auto' : 'visible';
                }
            });
            // Double for IE bug
            this.$container.offset(offset);
            this.$container.offset(offset);
            this._createiframe();
            var scrollObj = this._getCurrentLiDiv().scrollTop();
            if (scrollObj > this.$container.height() - this._getCurrentLiDiv().height() || scrollObj < 0) {
                this.$container.scrollTop(scrollObj);
            }
        },
        _hideList: function () {
            this.$container.hide();
            if (this.$iframe) {
                this.$iframe.detach();
            }
        },
        _getCurrentLiDiv: function () {
            var index = this.index;
            return (index > -1 && index < this.$listItems.length) ? $(this.$listItems.get(index)) : $();
        },
        _cancelSelection: function () {
            this._hideList();
            this.index = this._indexOf(this.liValues, this.value);
            this._hilightItem(0);
        },
        _selectItem: function (value) {
            var self = this;
            self._hideList();
            this.value = value;
            this.$select.find("option").filter(function () { return $(this).text() == value; }).attr("selected", true);
            this.$select.change();
            this.$textbox.val(value);
            this.index = self._indexOf(this.liValues, value);
            self._getCurrentLiDiv().addClass('esItemHover').siblings().removeClass('esItemHover');
            this.$textbox.focus().select();
        },
        _moveSelection: function (direction) {
            if (direction === 'down' && this.index < (this.$listItems.length - 1)) {
                this._hilightItem(+1);
            } else if (direction === 'up' && this.index > 0) {
                this._hilightItem(-1);
            }
        },
        _hilightItem: function (increment) {
            this.$listItems.removeClass('esItemHover');
            this.index = this.index + increment;
            this._getCurrentLiDiv().addClass('esItemHover');
        },
        _indexOf: function (array, value) {
            var length = array.length;
            for (var i = 0; i < length; i++) {
                if (array[i] === value) {
                    return i;
                }
            }
            return -1;
        },
        _createiframe: function () {
            if (!this.options.enableiframe) {
                return;
            }
            var offset = this.$container.offset();
            if (!this.$iframe) {
                this.$iframe = $('<iframe  src="javascript:false;"  tabindex="-1" frameborder="0" style="position:abolsute;' +
                                 ' display:inline; z-index=-1; filter:Alpha(opacity=\'0\'); width:' + this.$container.outerWidth() + 'px; " />');
            }
            this.$iframe.css({
                left: offset.left - 1,
                top: offset.top - 1,
                height: this.$container.outerHeight()
            });
            this.$iframe.appendTo(this.$container);
        }
    };
})(jQuery);

function myStringTrim(x) {
    return x.replace(/^\s+|\s+$/gm, '');
}