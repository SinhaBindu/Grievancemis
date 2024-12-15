(function ($) {
    // Define the fSelect jQuery plugin
    $.fn.fSelect = function (options) {
        if (typeof options === 'string') {
            // If options is a string, treat it as a method call
            var method = options;
            var args = Array.prototype.slice.call(arguments, 1); // Get all arguments after the method name
            return this.each(function () {
                var data = $(this).data('fSelect');
                if (data && typeof data[method] === 'function') {
                    data[method].apply(data, args);  // Pass the arguments to the method
                }
            });
        } else {
            // Default settings for the plugin
            var settings = $.extend({
                placeholder: 'Select some options',
                numDisplayed: 3,
                overflowText: '{n} selected',
                searchText: 'Search',
                showSearch: true
            }, options);

            // Initialize fSelect
            return this.each(function () {
                var data = $(this).data('fSelect');
                if (!data) {
                    // Initialize the fSelect instance if it doesn't already exist
                    data = new fSelect(this, settings);
                    $(this).data('fSelect', data);
                }
            });
        }
    };


    // Constructor for fSelect
    function fSelect(select, settings) {
        this.$select = $(select);
        this.settings = settings;
        this.create();
    }

    // Prototype for fSelect
    fSelect.prototype = {
        // Create the fSelect UI
        create: function () {
            var multiple = this.$select.is('[multiple]') ? ' multiple' : '';
            this.$select.wrap('<div class="fs-wrap' + multiple + '"></div>');
            this.$select.before('<div class="fs-label-wrap"><div class="fs-label">' + this.settings.placeholder + '</div><span class="fs-arrow"></span></div>');
            this.$select.before('<div class="fs-dropdown hidden"></div>');
            this.$select.addClass('hidden');
            this.reload();
        },

        // Reload the options dropdown
        reload: function () {
            var choices = '';
            if (this.settings.showSearch) {
                choices += '<div class="fs-search"><input type="search" placeholder="' + this.settings.searchText + '" /></div>';
            }
            choices += this.buildOptions(this.$select);
            this.$select.siblings('.fs-dropdown').html(choices);
            this.reloadDropdownLabel();
        },

        // Build the options from the select element
        buildOptions: function ($element) {
            var choices = '';
            var self = this;
            $element.children().each(function (i, el) {
                var $el = $(el);
                if ('optgroup' === $el.prop('nodeName').toLowerCase()) {
                    choices += '<div class="fs-optgroup">';
                    choices += '<div class="fs-optgroup-label">' + $el.prop('label') + '</div>';
                    choices += self.buildOptions($el);
                    choices += '</div>';
                } else {
                    var selected = $el.is('[selected]') ? ' selected' : '';
                    choices += '<div class="fs-option' + selected + '" data-value="' + $el.prop('value') + '"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">' + $el.html() + '</div></div>';
                }
            });
            return choices;
        },

        // Reload the dropdown label based on selected options
        reloadDropdownLabel: function () {
            var $wrap = this.$select.closest('.fs-wrap');
            var settings = this.settings;
            var labelText = [];

            $wrap.find('.fs-option.selected').each(function (i, el) {
                labelText.push($(el).find('.fs-option-label').text());
            });

            if (labelText.length < 1) {
                labelText = settings.placeholder;
            } else if (labelText.length > settings.numDisplayed) {
                labelText = settings.overflowText.replace('{n}', labelText.length);
            } else {
                labelText = labelText.join(', ');
            }

            $wrap.find('.fs-label').html(labelText);
            this.$select.trigger('change');
        },

        // Set the selected values programmatically
        setSelectedValues: function (values) {
            var selectedValues = Array.isArray(values) ? values : [values];

            var $wrap = this.$select.closest('.fs-wrap');
            var $options = $wrap.find('.fs-option');

            // Reset all options
            $options.removeClass('selected');

            // Select options based on values array
            selectedValues.forEach(function (value) {
                $options.filter('[data-value="' + value + '"]').addClass('selected');
            });

            this.reloadDropdownLabel();
        }
    };

    // Bind event listeners
    $(document).on('click touchstart', '.fs-label', function () {
        var $wrap = $(this).closest('.fs-wrap');
        $wrap.find('.fs-dropdown').toggleClass('hidden');
        $wrap.find('.fs-search input').focus();
    });

    $(document).on('click touchstart', '.fs-option', function () {
        var $this = $(this);
        var $wrap = $this.closest('.fs-wrap');

        if ($wrap.hasClass('multiple')) {
            $this.toggleClass('selected');
            var selected = [];
            $wrap.find('.fs-option.selected').each(function (i, el) {
                selected.push($(el).data('value'));
            });
        } else {
            var selected = $this.data('value');
            $wrap.find('.fs-option').removeClass('selected');
            $this.addClass('selected');
            $wrap.find('.fs-dropdown').addClass('hidden');
        }

        $wrap.find('select').val(selected);
        $wrap.find('select').fSelect('reloadDropdownLabel');
    });

    $(document).on('click touchstart', function (e) {
        var $wrap = $(e.target).closest('.fs-wrap');
        if ($wrap.length < 1) {
            $('.fs-dropdown').addClass('hidden');
        } else {
            var is_hidden = $wrap.find('.fs-dropdown').hasClass('hidden');
            $('.fs-dropdown').addClass('hidden');
            if (!is_hidden) {
                $wrap.find('.fs-dropdown').removeClass('hidden');
            }
        }
    });

    $(document).on('keyup', '.fs-search input', function () {
        var $wrap = $(this).closest('.fs-wrap');
        var keywords = $(this).val().trim();

        $wrap.find('.fs-option, .fs-optgroup-label').removeClass('hidden');

        if (keywords) {
            $wrap.find('.fs-option').each(function () {
                var regex = new RegExp(keywords, 'gi');
                if (!$(this).find('.fs-option-label').text().match(regex)) {
                    $(this).addClass('hidden');
                }
            });

            $wrap.find('.fs-optgroup-label').each(function () {
                var num_visible = $(this).closest('.fs-optgroup').find('.fs-option:not(.hidden)').length;
                if (num_visible < 1) {
                    $(this).addClass('hidden');
                }
            });
        }
    });

})(jQuery);
