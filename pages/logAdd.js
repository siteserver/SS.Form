var $url = '/pages/logAdd';
var $apiUrl = utils.getQueryString('apiUrl');
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $logId = utils.getQueryString('logId');
var $returnUrl = utils.getQueryString('returnUrl');

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: '',
  fieldInfoList: []
};

var methods = {
  load: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        logId: $logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.fieldInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

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
      siteId: $siteId,
      channelId: $channelId,
      contentId: $contentId,
      formId: $formId,
      logId: $logId
    };
    for (var i = 0; i < this.fieldInfoList.length; i++) {
      var style = this.fieldInfoList[i];
      payload[style.title] = style.value;
    }

    utils.loading(true);
    $api.post($url, payload).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "数据保存成功",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.btnNavClick("logs.html");
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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
    location.href = pageName + '?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  }
};

Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});