-->8
-- game logic stuff

function newpos()
 local x
 local pos={
  king_w=0,
  king_b=0,
  piece120={},
  color120={},
  enpas=-1,
  castleperm=0b1111,
  cancastle=-1,
  fiftymove=0,
  ply=0,
  poskey=0,
  side=true,
  history={}
 }

 --fill it with offboard values first
 for x = 0,119 do
  pos.piece120[x],pos.color120[x]=t_offboard,t_offboard
 end
 --now set empty squares
 for x = 0,63 do
  local mb=mailbox64[x]
  pos.piece120[mb],pos.color120[mb]=t_empty,t_empty
 end

 return pos
end

--makes a duplicate of a board position
function clonepos(origa)
 local pos,x=newpos()
 pos.king_w,pos.king_b,pos.enpas,pos.castleperm,pos.cancastle,pos.fiftymove,pos.ply,pos.poskey,pos.side=origa.king_w,origa.king_b,origa.enpas,origa.castleperm,origa.cancastle,origa.fiftymove,origa.ply,origa.poskey,origa.side
 for x = 0,119 do
  pos.piece120[x],pos.color120[x]=origa.piece120[x],origa.color120[x]
 end
 --note: this copies by reference
 --might be not what we want?
 if #origa.history>0 then
  for x = 1,#origa.history do
   pos.history[x]=origa.history[x]
  end
 end
 return pos
end

function genmove(list,from,to,kind,pos,promo)
 -- generates a move

 -- kind - indicating the type of move
 --      - 0 quiet (no capture)
 --      - 1 capture
 --      - 2 enpas
 --      - 3 king side castle
 --      - 4 qeen side castle
 local piece,side=pos.piece120[from],pos.color120[from]
 local mov={
  from=from,
  to=to,
  prevenpas=pos.enpas,
  cap=false,
  enpas=false,
  castle=0,
  cappiece=t_empty,
  prevcastleperm=pos.castleperm,
  prevcancastle=pos.cancastle,
  promo=t_empty,
  prevfifty=pos.fiftymove,
  val=0
 }
 if kind==1 then
  mov.cap,mov.cappiece=true,pos.piece120[to]
 elseif kind==2 then
  mov.cap,mov.enpas,mov.cappiece=true,true,t_pawn
 elseif kind==3 then
  mov.castle=bkcp
  if side==t_white then
   mov.castle=wkcp
  end
 elseif kind==4 then
  mov.castle=bqcp
  if side==t_white then
   mov.castle=wqcp
  end
 end
 if (promo!=nil) mov.promo=promo
 if (mov.cap) mov.val=1000+mvvlva[piece][mov.cappiece]
 if (mov.castle) mov.val+=90
 if (mov.promo==t_queen) mov.val+=900
 add(list,mov)
end

function startboard_classic(pos)
 local i
 for i=0,7 do
  restorepiece(pos,mailbox64[i+8],t_pawn,t_black)
  restorepiece(pos,mailbox64[i+48],t_pawn,t_white)
  restorepiece(pos,mailbox64[i],backrow[i+1],t_black)
  restorepiece(pos,mailbox64[i+56],backrow[i+1],t_white)
 end
 pos.side=t_white
end

function piecemoves(pos,loc120,list,attac)
 -- generates moves for a piece
 -- attac -> only capture moves
 local p,side,j,n,n2,col=pos.piece120[loc120],pos.color120[loc120]

 if p==0 then
  return
 elseif p!=t_pawn then
  --not pawn moves
  for j=1,pdirections[p] do
   n=loc120
   repeat
    n+=poffset[p][j]
    col=pos.color120[n]
    if col==t_empty and attac==false then
     --quiet move
     genmove(list,loc120,n,0,pos)
    elseif col==-side then
     --capture move
     genmove(list,loc120,n,1,pos)
    end
   until col!=t_empty or ponemove[p]==1
  end
  if p==t_king and attac==false then
   if (pos.cancastle==-1) checkcastle(pos)
   n=pos.cancastle
   if side==t_white then
    if (band(n,wkcp)==wkcp) genmove(list,loc120,97,3,pos)
    if (band(n,wqcp)==wqcp) genmove(list,loc120,93,4,pos)
   else
    if (band(n,bkcp)==bkcp) genmove(list,loc120,27,3,pos)
    if (band(n,bqcp)==bqcp) genmove(list,loc120,23,4,pos)
   end
  end
 else
  local promo,first=false,false
  --pawn moves
  if side==t_white then
   n=loc120-10
   n2=n-10
   first,promo=mailbox120y[loc120]==6,mailbox120y[n]==0
  else
   n=loc120+10
   n2=n+10
   first,promo=mailbox120y[loc120]==1,mailbox120y[n]==7
  end
  if pos.color120[n]==t_empty and attac==false then
   --quiet move
   if promo then
    addpromos(list,loc120,n,0,pos)
   else
    genmove(list,loc120,n,0,pos)
   end
   if first and pos.color120[n2]==t_empty then
    --quiet 2-step move
    genmove(list,loc120,n2,0,pos)
   end
  end
  if pos.color120[n-1]==-side then
   --capture move
   if promo then
    addpromos(list,loc120,n-1,1,pos)
   else
    genmove(list,loc120,n-1,1,pos)
   end
  elseif n-1==pos.enpas and attac==false then
   genmove(list,loc120,n-1,2,pos)
  end
  if pos.color120[n+1]==-side then
   --capture move
   if promo then
    addpromos(list,loc120,n+1,1,pos)
   else
    genmove(list,loc120,n+1,1,pos)
   end
  elseif n+1==pos.enpas and attac==false then
   genmove(list,loc120,n+1,2,pos)
  end
 end
end

function addpromos(list,from,to,kind,pos)
 genmove(list,from,to,kind,pos,t_queen)
 genmove(list,from,to,kind,pos,t_rook)
 genmove(list,from,to,kind,pos,t_bishop)
 genmove(list,from,to,kind,pos,t_knight)
end

function makemove(pos,mov)
 local side,piece,to,from=pos.color120[mov.from],pos.piece120[mov.from],mov.to,mov.from
 --set enpas
 if piece==t_pawn and mov.enpas==false and mov.cap==false then
  local dst=to-from
  if dst==-20 and side==t_white then
   pos.enpas=from-10
  elseif dst==20 and side==t_black then
   pos.enpas=from+10
  else
   pos.enpas=-1
  end
 else
  pos.enpas=-1
 end

 if mov.cap then
  if mov.enpas then
   if side==t_white then
    clearpiece(pos,to+10)
   else
    clearpiece(pos,to-10)
   end
  else
   clearpiece(pos,to)
  end
 end

 if mov.castle!=-1 then
  if (mov.castle==wkcp) movepiece(pos,98,96)
  if (mov.castle==wqcp) movepiece(pos,91,94)
  if (mov.castle==bkcp) movepiece(pos,28,26)
  if (mov.castle==bqcp) movepiece(pos,21,24)
 end

 if piece==t_king then
  if side==t_white then
   pos.castleperm=band(pos.castleperm,0b1100)
  else
   pos.castleperm=band(pos.castleperm,0b0011)
  end
 end
 
 if (from==98 or to==98) pos.castleperm=band(pos.castleperm,0b1110)
 if (from==91 or to==91) pos.castleperm=band(pos.castleperm,0b1101)
 if (from==28 or to==28) pos.castleperm=band(pos.castleperm,0b1011)
 if (from==21 or to==21) pos.castleperm=band(pos.castleperm,0b0111)
 
 pos.cancastle=-1
 movepiece(pos,from,to)
 if mov.promo!=t_empty then
  clearpiece(pos,to)
  restorepiece(pos,to,mov.promo,side)
 end
 pos.ply+=1
 pos.fiftymove+=1
 if (mov.cap or piece==t_pawn) pos.fiftymove=0
 pos.history[#pos.history+1]=mov
 pos.side=-pos.side
end

function takemove(pos)
 if (#pos.history==0) return
 local mov=pos.history[#pos.history]
 del(pos.history,mov)
 pos.enpas=mov.prevenpas

 local side=pos.color120[mov.to]
 if mov.promo!=t_empty then
  clearpiece(pos,mov.to)
  restorepiece(pos,mov.to,t_pawn,side)
 end

 movepiece(pos,mov.to,mov.from)
 if mov.cap then
  if mov.enpas then
   if pos.color120[mov.from]==t_white then
    restorepiece(pos,mov.to+10,t_pawn,t_black)
   else
    restorepiece(pos,mov.to-10,t_pawn,t_white)
   end
  else
   restorepiece(pos,mov.to,mov.cappiece,-pos.color120[mov.from])
  end
 end
 if mov.castle!=-1 then
  if (mov.castle==wkcp) movepiece(pos,96,98)
  if (mov.castle==wqcp) movepiece(pos,94,91)
  if (mov.castle==bkcp) movepiece(pos,26,28)
  if (mov.castle==bqcp) movepiece(pos,24,21)
 end
 pos.castleperm,pos.cancastle,pos.fiftymove,pos.side=mov.prevcastleperm,mov.prevcancastle,mov.prevfifty,-pos.side
end

function clearpiece(pos,loc120)
 if pos.piece120[loc120]!=t_empty then
  pos.poskey=addhash(pos.poskey,pos.piece120[loc120],pos.color120[loc120],loc120)
  pos.piece120[loc120],pos.color120[loc120]=t_empty,t_empty
 end

end

function movepiece(pos,from120,to120)
 local piece,side=pos.piece120[from120],pos.color120[from120]

 pos.poskey=addhash(pos.poskey,piece,side,from120)
 pos.poskey=addhash(pos.poskey,piece,side,to120)

 pos.piece120[to120],pos.color120[to120]=pos.piece120[from120],pos.color120[from120]
 pos.piece120[from120],pos.color120[from120]=t_empty,t_empty

 if pos.piece120[to120]==t_king then
  if pos.color120[to120]==t_white then
   pos.king_w=to120
  else
   pos.king_b=to120
  end
 end
end

function restorepiece(pos,loc120,pce,col)
 pos.poskey=addhash(pos.poskey,pce,col,loc120)

 pos.piece120[loc120],pos.color120[loc120]=pce,col
 if pos.piece120[loc120]==t_king then
  if pos.color120[loc120]==t_white then
   pos.king_w=loc120
  else
   pos.king_b=loc120
  end
 end
end

function getallmoves(pos,moves,side,attac)
 local x
 for x=21,98 do
  if pos.piece120[x]>0 and pos.color120[x]==side then
   piecemoves(pos,x,moves,attac)
  end
 end
end

function ischeck(pos,side)
 if side==t_white then
  return isattacked(pos,pos.king_w,t_black)
 else
  return isattacked(pos,pos.king_b,t_white)
 end
end

function isattacked(pos,loc,side)
 --pawn attacks
 local pwn1,pwn2
 if side==t_white then
  pwn1,pwn2=loc+11,loc+9
 else
  pwn1,pwn2=loc-11,loc-9
 end
 if (pos.color120[pwn1]==side and pos.piece120[pwn1]==t_pawn) return true
 if (pos.color120[pwn2]==side and pos.piece120[pwn2]==t_pawn) return true

 --rook, bishop and queen
 if (findatacker(pos,loc,side,t_rook)) return true
 if (findatacker(pos,loc,side,t_bishop)) return true

 --knight,king
 if (findatacker(pos,loc,side,t_knight)) return true
 if (findatacker(pos,loc,side,t_king)) return true

 return false
end

function findatacker(pos,loc,side,piece)
 local j,n,pce,col
 for j=1,pdirections[piece] do
  n=loc
  repeat
   n=n+poffset[piece][j]
   pce=pos.piece120[n]
   col=pos.color120[n]
   if (col==side and (pce==piece or (pce==t_queen and ponemove[piece]==0))) return true
  until col!=t_empty or ponemove[piece]==1
 end
 return false
end

function ismate(pos)
 local moves,m={}
 getallmoves(pos,moves,pos.side,false)
 if #moves>0 then
  for m in all(moves) do
   makemove(pos,m)
   if ischeck(pos,-pos.side)==false then
    takemove(pos)
    return false
   end
   takemove(pos)
  end
 end
 return true
end

function winconditions()
 --if player begins the turn checking the other one
 --we screwed up and its actually checkmate
 if ischeck(board,-board.side) then
  showmate(-board.side==t_white)
  return
 end

 if board.fiftymove >= 100 then
  itext("draw by fifty-move rule")
  showdraw()
  return
 end

 --now the regular check
 check=ischeck(board,board.side)
 if check then
  if ismate(board) then
   showmate(board.side==t_white)
   return
  else
   showcheck(board.side==t_white)
   return
  end
 end

 --now stalemate
 local moves={}
 getallmoves(board,moves,board.side,false)
 purgebadmoves(board,moves)
 if #moves==0 then
  itext("stalemate")
  showdraw()
 end

end

--removes discovered checks and mates
function purgebadmoves(pos,movs)
 local i,m
 for i=#movs,1,-1 do
  m=movs[i]
  makemove(pos,m)
  if (ischeck(pos,-pos.side)) del(movs,m)
  takemove(pos)
 end
end

function getbestmove()
 cpu_time_start,bestmove,peevee=time(),nil,{}

 local maxdepth,pos,j,v=3,clonepos(board)
 cpu_leaf1,cpu_leaf2=0,0

 for j=0,maxdepth do
  v=alphabeta(j,pos,-32767,32767)
  bestmove=peevee[pos.poskey]
  if (bestmove!=nil) setcpucur(bestmove.from)
  --logcutoffpurge()
 end
 cputhinking,cpumove=false,bestmove
 cpu_time_end=time()
 if (cpumove==nil) winconditions()
 --logcutoffpurge()
end

function alphabeta(depth,pos,alpha,beta)
 local side,moves,oldalpha,score,bestmove,i,m=pos.side,{},alpha

 if (pos.fiftymove >= 100) return 0

 if depth<=0 then
  addleaf()
  --return evalpos(pos)*side
  return quiescence(pos,alpha,beta)
 end

 getallmoves(pos,moves,side,false)
 purgebadmoves(pos,moves)

 if #moves==0 then
  if ischeck(pos,side) then
   return -mate
  else
   return 0
  end
 end
 awardpv(moves,pos.poskey)
 for i=1,#moves do
  m=picknextmove(moves,i)
  makemove(pos,moves[i])
  score=-alphabeta(depth-1,pos,-beta,-alpha)
  takemove(pos)
  if score>alpha then
   if (score>=beta) return beta
   alpha,bestmove=score,m
  end
 end
 if (oldalpha!=alpha) peevee[pos.poskey]=bestmove
 if (stat(1)>=0.87) yield()
 return alpha
end

function quiescence(pos,alpha,beta)
 local score,side,moves,oldalpha,bestmove,i,m=evalpos(pos)*pos.side,pos.side,{}

 addleaf()
 --check reptetition
 if (score>=beta) return beta
 if (score>alpha) alpha=score

 getallmoves(pos,moves,side,true)
 purgebadmoves(pos,moves)
 oldalpha=alpha
 awardpv(moves,pos.poskey)
 for i=1,#moves do
  m=picknextmove(moves,i)
  makemove(pos,moves[i])
  score=-quiescence(pos,-beta,-alpha)
  takemove(pos)
  if score>alpha then
   if (score>=beta) return beta
   alpha,bestmove=score,m
  end
 end
 if (oldalpha!=alpha) peevee[pos.poskey]=bestmove
 if (stat(1)>=0.87) yield()
 return alpha
end


function picknextmove(lst,i)
 local best,j,tmp,bestnum=-1

 for j=i,#lst do
  if lst[j].val>best then
   best,bestnum=lst[j].val,j
  end
 end
 tmp=lst[i]
 lst[i]=lst[bestnum]
 lst[bestnum]=tmp
 return lst[i]
end

function checkcastle(pos)
 local ret,prms,nprms,i,j,fp,s=pos.castleperm,{wkcp,wqcp,bkcp,bqcp},{0b1110,0b1101,0b1011,0b0111}
 local freepos={
  {96,97},
  {92,93,94},
  {26,27},
  {22,23,24}
 }
 local atcpos={
  {95,96,97},
  {95,93,94},
  {25,26,27},
  {25,23,24}
 }

 for i=1,4 do
  if band(ret,prms[i])==prms[i] then
   fp=freepos[i]
   for j=1,#fp do
    if pos.piece120[fp[j]]!=t_empty then
     ret=band(ret,nprms[i])
     break
    end
   end
  end
 end
 if ret==0b0000 then
  pos.cancastle=ret
  return
 end

 for i=1,4 do
  if band(ret,prms[i])==prms[i] then
   fp=atcpos[i]
   for j=1,#fp do
    s=t_black
    if (i>2) s=t_white
    if isattacked(pos,fp[j],s) then
     ret=band(ret,nprms[i])
     break
    end
   end
  end
 end
 pos.cancastle=ret
end

function addleaf()
 cpu_leaf1+=1
 if cpu_leaf1>=2000 then
  cpu_leaf2+=1
  cpu_leaf1-=2000--used to be 10000
 end
end

function addhash(key,piece,side,loc)
 local i=piece*loc
 if (side==t_black) i=(piece+6)*loc
 return bxor(key,piecekeys[i])
end

function genkeys()
 local i
 srand(1)
 piecekeys={}
 for i=1,12*119 do
  piecekeys[i]=rnd(-1)
 end
end

function awardpv(list,poskey)
 local i,pvm,m
 pvm=peevee[poskey]
 if pvm!=nil then
  for i=1,#list do
   m=list[i]
   if (m.from==pvm.from and m.to==pvm.to and m.promo==pvm.promo) m.val=32000
  end
 end
end
-->8
--update

function update_game()
 --cursor animations
 curani()
 update_itext()
 update_thinkdts()
 if uimode=="ani" then
  update_ani()
 elseif uimode=="over" or uimode=="gmenu" then
  update_gmenu()
 -- if btnp(4) or btnp(5) then
 --  fadeout()
 --  show_menu()
 -- end
 else
  if cputurn() then
   update_cpu()
  else
   if uimode=="promo" then
    update_promo_h()
   else
    update_human()
   end
  end
  -- regenerate moves if new piece
  if movesdirty then
   movesdirty,tmpmoves=false,{}
   piecemoves(board,mailbox64[sel_x+sel_y*8],tmpmoves,false)
   if (board.color120[mailbox64[sel_x+sel_y*8]]==board.side) purgebadmoves(board,tmpmoves)
   makemovedots(tmpmoves)
  end
 end
 update_bigtext()
 if (igm) update_igm()
end