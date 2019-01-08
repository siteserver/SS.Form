var $url = '/pages/templatesLayerEdit';

var data = {
  siteId: utils.getQueryString('siteId'),
  apiUrl: utils.getQueryString('apiUrl'),
  type: utils.getQueryString('type'),
  name: utils.getQueryString('name'),
  pageLoad: false,
  pageAlert: null,
  templateInfo: null
};

var methods = {
  load: function () {
    var $this = this;
    $api.get($url + '?siteId=' + this.siteId + '&name=' + this.name).then(function (response) {
      var res = response.data;
      $this.templateInfo = res.value;
      if ($this.type == 'clone') {
        $this.templateInfo.name = '';
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);
        if ($this.type == 'clone') {
          $api.post($url + '?siteId=' + $this.siteId, {
            nameToClone: $this.name,
            name: $this.templateInfo.name,
            description: $this.templateInfo.description
          }).then(function (response) {
            parent.location.reload(true);
          }).catch(function (error) {
            $this.pageAlert = utils.getPageAlert(error);
          }).then(function () {
            utils.loading(false);
          });
        } else {
          $api.put($url + '/' + $this.departmenteId + '?siteId=' + $this.siteId, {
            departmentName: $this.departmentInfo.departmentName,
            userNames: $this.userNames.join(','),
            taxis: $this.departmentInfo.taxis
          }).then(function (response) {
            parent.location.reload(true);
          }).catch(function (error) {
            $this.pageAlert = utils.getPageAlert(error);
          }).then(function () {
            utils.loading(false);
          });
        }
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});