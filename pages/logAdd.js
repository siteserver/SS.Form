config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.formId = utils.getQueryString('formId');
config.logId = utils.getQueryString('logId');
config.returnUrl = utils.getQueryString('returnUrl');

var $api = new utils.Api('/ss.form/logs/add');

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: '',
  fieldInfoList: [],
};

var methods = {
  getValue: function (attributeName) {
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      if (style.attributeName === attributeName) {
        return style.value;
      }
    }
    return '';
  },

  setValue: function (attributeName, value) {
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      if (style.attributeName === attributeName) {
        style.value = value;
      }
    }
  },

  submit: function () {
    var $this = this;

    var payload = {
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      logId: config.logId
    };
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      // if (typeof style.value === 'object') {
      //   style.value = JSON.stringify(style.value);
      // }
      // console.log(style.value);
      payload[style.title] = style.value;
    }

    utils.loading(true);
    $api.post(payload, function (err, res) {
      utils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      alert({
        toast: true,
        type: 'success',
        title: "数据保存成功",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.btnNavClick("logs.html");
      });
    });
  },

  btnLayerClick: function (options) {
    this.pageAlert = null;
    var url = "pages/contentAddLayer" +
      options.name +
      ".html?siteId=" +
      this.site.id +
      "&channelId=" +
      this.channel.id;

    if (options.contentId) {
      url += "&contentId=" + options.contentId
    }

    if (options.args) {
      _.forIn(options.args, function (value, key) {
        url += "&" + key + "=" + encodeURIComponent(value);
      });
    }

    utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnImageClick: function (imageUrl) {
    top.utils.openImagesLayer([imageUrl]);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&apiUrl=' + encodeURIComponent(config.apiUrl) + '&returnUrl=' + encodeURIComponent(config.returnUrl);
  }
};

Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      logId: config.logId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.fieldInfoList = res.value;
      $this.pageLoad = true;
    });
  }
});