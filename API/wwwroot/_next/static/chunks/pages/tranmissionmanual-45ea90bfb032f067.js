(self.webpackChunk_N_E=self.webpackChunk_N_E||[]).push([[172],{19319:function(e,n,l){(window.__NEXT_P=window.__NEXT_P||[]).push(["/tranmissionmanual",function(){return l(48049)}])},48049:function(e,n,l){"use strict";l.r(n),l.d(n,{default:function(){return Z}});var a=l(85893),i=l(73762),t=l(29630),s=l(30120),r=l(79072),o=l(91655),d=l(70918),c=l(91359),u=l(65828),m=l(87536),x=l(88767),h=l(13136),g=l(67294),p=l(51593),v=l(76429),j=l(2676),f=l(43170),C=l(37024);const b=e=>{let{}=e;const n=(0,C.q)(),[l,i]=(0,g.useState)("lists"),[b,Z]=(0,g.useState)(null),{reset:I,control:P,watch:F,handleSubmit:w,setValue:D}=((0,g.useCallback)((e=>{i(e)}),[]),(0,m.cI)({defaultValues:{pageSize:10,pageIndex:1,searchContent:""}})),O=F(),{data:y,isLoading:W,isFetching:L}=(0,x.useQuery)(["documentsAllQuery",5,O.brandID,O.eModelID,O.LineOffID,O.searchContent,O.pageIndex,O.pageSize],(()=>p.xl.getAll({...O,systemFileCategory:"5"})),{keepPreviousData:!0,refetchOnWindowFocus:!1}),N=((0,g.useMemo)((()=>(null===y||void 0===y?void 0:y.data.items)||[]),[null===y||void 0===y?void 0:y.data]),(0,j.Nn)()),_=(0,g.useMemo)((()=>{var e;return(null===N||void 0===N||null===(e=N.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[N]),M=(0,j.bP)(O.brandID),S=(0,g.useMemo)((()=>{var e;return(null===M||void 0===M||null===(e=M.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[M]),T=(0,j.AP)(O.eModelID),k=(0,g.useMemo)((()=>{var e;return(null===T||void 0===T||null===(e=T.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[T]);(0,g.useMemo)((()=>[{Header:"T\xean folder",accessor:"name",minWidth:250,Filter:()=>null},{Header:"H\xe3ng xe",accessor:"brandID",Cell:e=>{let{row:n}=e;return(0,a.jsx)(a.Fragment,{children:n.original.brandName})},Filter:e=>{let{column:n}=e;return(0,a.jsx)(f.fD,{column:n,options:_})},minWidth:250},{Header:"D\xf2ng xe",accessor:"eModelID",Cell:e=>{let{row:n}=e;return(0,a.jsx)(a.Fragment,{children:n.original.emodelName})},Filter:e=>{let{column:n}=e;return(0,a.jsx)(f.fD,{column:n,options:S})},minWidth:250},{Header:"\u0110\u1eddi xe",accessor:"lineOffName",minWidth:250},{Header:"S\u1ed1 folders con",accessor:"countChildFile",align:"right",Filter:()=>null}]),[N]);return(0,a.jsxs)("div",{children:[(0,a.jsxs)("div",{className:"mb-[36px] flex justify-between items-center",children:[(0,a.jsxs)("div",{children:[(0,a.jsx)(t.Z,{variant:"largeTitle",children:"Qu\u1ea3n l\xfd t\xe0i li\u1ec7u transmission manual"}),(0,a.jsx)(t.Z,{children:"T\u1ea5t c\u1ea3 th\xf4ng tin li\xean quan \u0111\u1ebfn t\xe0i li\u1ec7u transmission manual \u0111\u01b0\u1ee3c qu\u1ea3n l\xfd t\u1eeb \u0111\xe2y "})]}),(0,a.jsx)(v.HS,{title:"T\u1ea1o m\u1edbi th\u01b0\u01a1ng hi\u1ec7u",handleFunction:n.onOpen})]}),(0,a.jsxs)(s.Z,{className:"mb-[36px] flex justify-between items-center",children:[(0,a.jsxs)(s.Z,{className:"h-full flex flex-wrap gap-2",children:[(0,a.jsx)(s.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:_,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"brandID",placeholder:"L\u1ecdc h\xe3ng xe",onChange:()=>{D("eModelID",""),D("LineOffID","")}})}),(0,a.jsx)(s.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:S,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"eModelID",placeholder:"L\u1ecdc d\xf2ng xe",onChange:()=>{D("LineOffID","")}})}),(0,a.jsx)(s.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:k,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"LineOffID",placeholder:"L\u1ecdc theo n\u0103m"})})]}),(0,a.jsx)(s.Z,{})]}),(0,a.jsxs)(s.Z,{sx:{mt:"36px"},children:[W||L?(0,a.jsxs)(r.ZP,{container:!0,spacing:"27px",children:[(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(o.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-1"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(o.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-2"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(o.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-3"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(o.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-4")]}):(null===y||void 0===y?void 0:y.data.items.length)?(0,a.jsx)(s.Z,{children:(0,a.jsx)(r.ZP,{container:!0,spacing:"27px",children:y.data.items.map(((e,n)=>{const l=e.id==b;return(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(h.GF,{handleClick:()=>{e.id==b?Z(null):Z(e.id)},data:e,isCollapsed:l,systemFileCategory:"5"})},"WiringFolderCollapseCard-".concat(n))}))})}):null,(0,a.jsx)(d.Z,{sx:{marginTop:"27px"},children:(0,a.jsx)(c.Z,{sx:{padding:"18px !important"},children:(0,a.jsx)(s.Z,{children:(0,a.jsx)(u.Z,{rowsPerPageOptions:[2,5,10,25,50],component:"div",count:(null===y||void 0===y?void 0:y.data.totalItem)||0,size:"small",page:(null===O||void 0===O?void 0:O.pageIndex)?O.pageIndex-1:0,onPageChange:(e,n)=>{D("pageIndex",n+1)},labelRowsPerPage:"S\u1ed1 folder m\u1ed7i trang",labelDisplayedRows:e=>{let{from:n,to:l,count:i,page:t}=e;return(0,a.jsx)("p",{children:"".concat(n,"\u2013").concat(l," c\u1ee7a ").concat(-1!==i?i:"nhi\u1ec1u h\u01a1n ".concat(l))})},rowsPerPage:O.pageSize||0,onRowsPerPageChange:e=>{D("pageSize",parseInt(e.target.value))}})})})})]}),(0,a.jsx)(h.$R,{isOpen:n.isOpen,onClose:n.onClose,systemFileCategory:"5"})]})};b.title="Qu\u1ea3n l\xfd Transmission Manual",b.getLayout=e=>(0,a.jsx)(i.Z,{children:e});var Z=b}},function(e){e.O(0,[587,109,491,211,412,59,29,136,774,888,179],(function(){return n=19319,e(e.s=n);var n}));var n=e.O();_N_E=n}]);