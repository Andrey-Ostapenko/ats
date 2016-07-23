begin
  :result := integratorcommissions_pkg.getintegratorcommission(p_err_msg => :p_err_msg,
                                                               p_integrator_commis => :p_integrator_commis,
                                                               p_gateway_id => :p_gateway_id,
                                                               p_operator_id => :p_operator_id,
                                                               p_paymentdate => :p_paymentdate,
                                                               p_amount => :p_amount,
                                                               p_client_commis => :p_client_commis);
end;