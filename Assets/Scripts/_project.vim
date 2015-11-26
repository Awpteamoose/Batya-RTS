let g:ag_prg='ag -S --nocolor --nogroup --column --noheading ' .
	\'--ignore "*.meta" '.
	\'--ignore ".git" '

let g:ctrlp_user_command = g:ag_prg . '-l -g "" %s'
