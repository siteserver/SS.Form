var $url = '/pages/templates';
var $urlHtml = '/pages/templates/html';

var data = {
  siteId: utils.getQueryString('siteId'),
  formId: utils.getQueryString('formId'),
  apiUrl: utils.getQueryString('apiUrl'),
  pageConfig: null,
  pageAlert: null,
  pageType: 'loading',
  templateInfoList: null,
  name: null,
  templateHtml: null,
};

var methods = {
  getIconUrl: function (templateInfo) {
    return '../templates/' + templateInfo.name + '/' + templateInfo.icon;
  },

  loadTemplates: function () {
    var $this = this;

    if (this.pageLoad) {
      utils.loading(true);
    }

    $api.get($url + '?siteId=' + this.siteId).then(function (response) {
      var res = response.data;

      $this.templateInfoList = res.value;
      $this.pageType = 'list';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
      $this.pageLoad = true;
    });
  },

  btnPreviewClick: function (name) {
    var $this = this;
    utils.openLayer({
      title: '预览模板',
      url: 'templatesLayerPreview.html?siteId=' + $this.siteId + '&name=' + name + '&apiUrl=' + encodeURIComponent($this.apiUrl)
    });
  },

  btnCloneClick: function (name) {
    var $this = this;
    utils.openLayer({
      title: '克隆模板',
      url: 'templatesLayerEdit.html?siteId=' + $this.siteId + '&type=clone&name=' + name + '&apiUrl=' + encodeURIComponent($this.apiUrl)
    });
  },

  btnDeleteClick: function (template) {
    var $this = this;
    utils.alertDelete({
      title: '删除模板',
      text: '此操作将删除模板' + template.name + '，确认吗？',
      callback: function () {
        utils.loading(true);
        $api.delete($url + '?siteId=' + this.siteId, {
          params: {
            name: template.name
          }
        }).then(function (response) {
          var res = response.data;

          $this.templateInfoList = res.value;
          $this.pageType = 'list';
        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          utils.loading(false);
        });
      }
    });
  },

  btnHtmlClick: function (template) {
    var $this = this;
    this.name = template.name;
    utils.loading(true);
    $api.get($urlHtml + '?siteId=' + this.siteId + '&name=' + this.name).then(function (response) {
      var res = response.data;

      $this.templateHtml = res.value;
      $this.pageType = 'edit';
      setTimeout(function () {
        $('.js-copytextarea').css({
          height: $(document).height() - 180
        });
      }, 100);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    utils.loading(true);
    $api.post($url + '?siteId=' + this.siteId, {
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "模板编辑成功！",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.pageType = 'list';
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.loadTemplates();
  }
});