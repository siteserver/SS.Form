var $url = '/pages/templatesLayerPreview';

var data = {
  siteId: utils.getQueryString('siteId'),
  apiUrl: utils.getQueryString('apiUrl'),
  type: utils.getQueryString('type'),
  name: utils.getQueryString('name'),
  pageLoad: false,
  pageAlert: null,
  templateInfo: null,
  formInfoList: null
};

var methods = {
  load: function () {
    var $this = this;
    $api.get($url, {
      params: {
        siteId: this.siteId,
        type: this.type,
        name: this.name
      }
    }).then(function (response) {
      var res = response.data;
      $this.templateInfo = res.value;
      $this.formInfoList = res.formInfoList;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  getPreviewUrl: function (formInfo) {
    return '../templates/' + this.templateInfo.name + '/' + this.templateInfo.main + '?siteId=' + this.siteId + '&formId=' + formInfo.id + '&apiUrl=' + encodeURIComponent(this.apiUrl);
  },
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});