;var handlers_mob_selected = function(){
    return {
         CHANGE:function(e){
            var val = this.get(e.field),
                viewer = _nexu_.View.Controller.Widget();//,
                //logos = _nexu_.LogoCarousel;
            switch(e.field){
                case 'Menu':

                break;
                case 'Location':

                break;
                case 'View':
                    var v = _.findWhere(_nexu_.View.Views,{Id:val});
                    _nexu_.View.Controller.ChangeView(v,e.sender.Menu);
                break;
            }
         }
    }

};